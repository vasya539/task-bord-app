using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Models;
using WebApi.Interfaces.IRepositories;
using WebApi.Interfaces.IServices;
using WebApi.Data.DTOs;
using AutoMapper;
using WebApi.BLs.Interfaces;

namespace WebApi.BLs
{
    public class ProjectBl : IProjectBl
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public ProjectBl(IProjectRepository projectRepository, IMapper mapper)
        {
            _mapper = mapper;
            _projectRepository = projectRepository;
        }

        public async Task<List<ProjectDto>> GetAllAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            List<ProjectDto> projectsDTOs = new List<ProjectDto>();
            foreach(var i in projects)
            {
                var normal = _mapper.Map<ProjectDto>(i);
                projectsDTOs.Add(normal);
            }

            return projectsDTOs;
        }

        public async Task Create(ProjectDto project)
        {
            var normalProject = _mapper.Map<Project>(project);
            await _projectRepository.CreateAsync(normalProject);
        }

        public async Task<ProjectDto> Read(int id)
        {
            var project = await _projectRepository.ReadAsync(id);
            var DTOProject = _mapper.Map<ProjectDto>(project);

            return DTOProject;
        }

        public async Task Update(ProjectDto project)
        {
            var newProject = _mapper.Map<Project>(project);

                try
                {
                    await _projectRepository.UpdateAsync(newProject);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return;
                }

            return;
        }

        public async Task Delete(int id)
        {
                await _projectRepository.DeleteAsync(id);
        }

    }
}
