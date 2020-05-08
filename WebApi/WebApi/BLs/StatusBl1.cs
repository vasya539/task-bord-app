using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Models;
using WebApi.Repositories;
using WebApi.Data.DTOs;
using WebApi.BLs.Interfaces;
using AutoMapper;
using WebApi.Repositories.Interfaces;

namespace WebApi.BLs
{
    public class StatusBl : IStatusBl
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IMapper _mapper;


        public StatusBl(IStatusRepository statusRepository, IMapper mapper)
        {
            _mapper = mapper;
            _statusRepository = statusRepository;
        }
        public async Task Create(StatusDto statusDto)
        {
            statusDto.Id = 0;
            var normalStatus = _mapper.Map<Status>(statusDto);
            await _statusRepository.CreateAsync(normalStatus);
        }

        public async Task Delete(int id)
        {
            try
            {
                await _statusRepository.DeleteAsync(id);
            }
            catch (Exception)
            {

                return;
            }
        }

        public async Task<List<StatusDto>> GetAllAsync()
        {
            var statuses = await _statusRepository.GetAllAsync();
            List<StatusDto> statusesDtos = new List<StatusDto>();
            foreach (var i in statuses)
            {
                var normal = _mapper.Map<StatusDto>(i);
                statusesDtos.Add(normal);
            }
            return statusesDtos;
        }

        public async Task<StatusDto> Read(int id)
        {
            var status = await _statusRepository.ReadAsync(id);
            var statusDto = _mapper.Map<StatusDto>(status);
            return statusDto;
        }

        public async Task Update(StatusDto status)
        {
            var newStatus = _mapper.Map<Status>(status);
            try
            {
                await _statusRepository.UpdateAsync(newStatus);

            }
            catch (DbUpdateConcurrencyException)
            {
                return;
            }
            return;
        }
    }
}
