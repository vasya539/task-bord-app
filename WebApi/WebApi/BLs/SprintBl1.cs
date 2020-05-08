using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Models;
using WebApi.Repositories;
using WebApi.BLs.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.BLs.Communication;
using AutoMapper;
using WebApi.Data.DTOs;

namespace WebApi.BLs
{
    public class SprintBl : ISprintBl
    {
        private readonly ISprintRepository _sprintRepository;
        private readonly IMapper _mapper;
        public SprintBl(ISprintRepository sprintRepository, IMapper mapper)
        {
            _sprintRepository = sprintRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<SprintDto>> GetAllAsync()
        {
            IEnumerable<Sprint> sprints = await _sprintRepository.GetAllAsync();
            IEnumerable<SprintDto> dto = _mapper.Map<IEnumerable<Sprint>, IEnumerable<SprintDto>>(sprints);
            return dto;
        }
        public async Task<SprintDto> GetByIdAsync(int id)
        {
            Sprint sprint = await _sprintRepository.GetByIdAsync(id);
            SprintDto sprintDTO = _mapper.Map<Sprint, SprintDto>(sprint);
            return sprintDTO;
        }
        public async Task<SprintResponse> CreateAsync(SaveSprintDto dto)
        {
            try
            {
                Sprint sprint = _mapper.Map<SaveSprintDto, Sprint>(dto);
                await _sprintRepository.CreateAsync(sprint);
                var sprintDTO = _mapper.Map<Sprint, SprintDto>(sprint);
                return new SprintResponse(sprintDTO);
            }
            catch (Exception ex)
            {
                return new SprintResponse($"An error occurred when saving the sprint: {ex.Message}");
            }
        }
        public async Task<SprintResponse> UpdateAsync(SprintDto dto)
        {
            var existingSprint = await _sprintRepository.GetByIdAsync(dto.Id);

            if (existingSprint == null)
                return new SprintResponse("Sprint not found.");

            existingSprint.StartDate = dto.StartDate;
            existingSprint.ExpireDate = dto.ExpireDate;

            try
            {
                await _sprintRepository.UpdateAsync(existingSprint);
                SprintDto sprintDTO = _mapper.Map<Sprint, SprintDto>(existingSprint);
                return new SprintResponse(sprintDTO);
            }
            catch (Exception ex)
            {
                return new SprintResponse($"An error occurred when updating the sprint: {ex.Message}");
            }
        }
        public async Task<SprintResponse> DeleteAsync(int id)
        {
            Sprint existingSprint = await _sprintRepository.GetByIdAsync(id);

            if (existingSprint == null)
                return new SprintResponse("Sprint not found.");

            try
            {
                await _sprintRepository.DeleteAsync(existingSprint);
                var sprintDTO = _mapper.Map<Sprint, SprintDto>(existingSprint);
                return new SprintResponse(sprintDTO);
            }
            catch (Exception ex)
            {
                return new SprintResponse($"An error occurred when deleting the sprint: {ex.Message}");
            }
        }        
    }
}
