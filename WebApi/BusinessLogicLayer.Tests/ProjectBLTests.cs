using AutoMapper;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.BLs;
using WebApi.BLs.Communication;
using WebApi.Data.DTOs;
using WebApi.Data.Models;
using WebApi.Exceptions;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories.Interfaces;
using Xunit;

namespace BusinessLogicLayer.Tests
{
    public class ProjectBLTests
    {
        private readonly Mock<IProjectRepository> mockProjRepo;
        private readonly Mock<ISprintRepository> mockSprintRepo;
        private readonly IMapper mapper;

        private readonly Mock<IProjectUserRepository> mockProjectUserRepo;

        public ProjectBLTests()
        {
            mockProjRepo = new Mock<IProjectRepository>();
            mockSprintRepo = new Mock<ISprintRepository>();
            mockProjectUserRepo = new Mock<IProjectUserRepository>();

            var mapperConfig = new MapperConfiguration(cf => cf.AddProfile(new WebApi.Data.Profiles.AutoMapperProfiler()));
            mapper = mapperConfig.CreateMapper();

        }

        [Fact]
        public void GetAllAsync_Should_Returns_AllProjects()
        {
            // Arrange
            mockProjRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(GetSampleProjects());
            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var resultFromBl = projectBL.GetAllAsync();
            // Assert
            var typeResult = Assert.IsType<Task<IEnumerable<ProjectDto>>>(resultFromBl);
            var model = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(typeResult.Result);
            Assert.Equal(GetSampleProjects().Count, model.Count());
        }

        [Fact]
        public async Task GetByIdAsync_Should_Fail_Access()
        {
            // Arrange   
            mockProjRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(GetSampleProjects()[0]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(AppUserRole.None);
            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var resultFromBl = projectBL.GetByIdAsync(It.IsAny<int>(), It.IsAny<string>());
            // Assert
            await Assert.ThrowsAsync<ForbiddenResponseException>(() => projectBL.GetByIdAsync(It.IsAny<int>(), It.IsAny<string>()));
        }
        [Theory]
        [InlineData("IsObserver")]
        [InlineData("IsDeveloper")]
        [InlineData("IsScrumMaster")]
        [InlineData("IsOwner")]
        public void GetByIdAsync_Should_Work_Can_Access(string userIdRole)
        {
            // Arrange   
            mockProjRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(GetSampleProjects()[0]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember("IsObserver", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember("IsDeveloper", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember("IsScrumMaster", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember("IsOwner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var resultFromBl = projectBL.GetByIdAsync(1, userIdRole);
            // Assert
            mockProjRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(999)]
        public async Task GetByIdAsync_Should_Throw_NotFoundResponseException(int projectId)
        {
            string userId = It.IsAny<string>();
            // Arrange   
            mockProjRepo.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(null as Project);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(userId, projectId)).ReturnsAsync(AppUserRole.Owner);
            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var resultFromBl = projectBL.GetByIdAsync(projectId, userId);
            // Assert
            await Assert.ThrowsAsync<NotFoundResponseException>(() => projectBL.GetByIdAsync(projectId, userId));
        }

        [Fact]
        public void GetByIdAsync_Should_Call_Once_ReadAsync()
        {
            string userId = It.IsAny<string>();
            int projectId = 1;
            int index = projectId - 1;
            // Arrange   
            mockProjRepo.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(GetSampleProjects()[index]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(userId, projectId)).ReturnsAsync(AppUserRole.Owner);
            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var resultFromBl = projectBL.GetByIdAsync(projectId, userId);
            // Assert
            var typeResult = Assert.IsType<Task<ProjectDto>>(resultFromBl);
            mockProjRepo.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        }

        [Fact]
        public void GetByIdAsync_Should_Return_The_Same_Resault()
        {
            // Arrange   
            string userId = It.IsAny<string>();
            int projectId = 1;
            int index = projectId - 1;

            mockProjRepo.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(GetSampleProjects()[index]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(userId, projectId)).ReturnsAsync(AppUserRole.Owner);
            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var resultFromBl = projectBL.GetByIdAsync(projectId, userId);
            // Assert
            Assert.Equal(GetSampleProjects()[index].Id, resultFromBl.Result.Id);
            Assert.Equal(GetSampleProjects()[index].Name, resultFromBl.Result.Name);
            Assert.Equal(GetSampleProjects()[index].Description, resultFromBl.Result.Description);
        }

        [Fact]
        public void Create_Should_Call_Once_CreateAsync()
        {
            // Arrange   
            var projectDto = new ProjectDto { Id = 1, Name = "Project 1", Description = "Some description" };
            mockProjRepo.Setup(repo => repo.CreateAsync(null as Project));
            mockSprintRepo.Setup(repo => repo.CreateAsync(null as Sprint));
            mockProjectUserRepo.Setup(repo => repo.CreateRecordAsync(null as ProjectUser));

            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var result = projectBL.CreateAsync(projectDto, It.IsAny<string>());
            // Assert
            mockProjRepo.Verify(r => r.CreateAsync(It.IsAny<Project>()), Times.Once);
            Assert.IsType<ProjectResponse>(result.Result);
            Assert.True(result.Result.Success);
        }

        [Theory]
        [InlineData("IsObserver")]
        [InlineData("IsDeveloper")]
        [InlineData("IsScrumMaster")]
        [InlineData("IsNone")]
        public async Task Update_Should_Fail_Only_Owner_Can_Update(string userIdRole)
        {
            // Arrange   
            var projectDto = new ProjectDto { Id = 1, Name = "First Update", Description = "Some description for Project1" };
            mockProjRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetSampleProjects()[0]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember("IsObserver", 1)).ReturnsAsync(AppUserRole.Observer);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember("IsDeveloper", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember("IsScrumMaster", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember("IsNone", It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var result = projectBL.UpdateAsync(projectDto, userIdRole);
            // Assert
            mockProjRepo.Verify(r => r.UpdateAsync(It.IsAny<Project>()), Times.Never);
            await Assert.ThrowsAsync<ForbiddenResponseException>(() => projectBL.UpdateAsync(projectDto, userIdRole));
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(999)]
        public void UpdateAsync_Should_Not_Found(int projectId)
        {   
            // Arrange   
            string userId = It.IsAny<string>();
            var projectDto = new ProjectDto { Id = projectId, Name = "First Update", Description = "Some description for Project1" };
   
            mockProjRepo.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(null as Project);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(userId, projectId)).ReturnsAsync(AppUserRole.Owner);
            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var result = projectBL.UpdateAsync(projectDto, userId);
            // Assert
            Assert.IsType<ProjectResponse>(result.Result);
            Assert.False(result.Result.Success);
            Assert.Equal("Project not found", result.Result.Message);
        }

        [Fact]
        public void Update_Should_Call_Once_UpdateAsync()
        {
            // Arrange   
            var projectDto = new ProjectDto { Id = 1, Name = "First Update", Description = "Some description for Project1" };
            mockProjRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetSampleProjects()[0]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);

            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var result = projectBL.UpdateAsync(projectDto, It.IsAny<string>());
            // Assert
            mockProjRepo.Verify(r => r.UpdateAsync(It.IsAny<Project>()), Times.Once);
            Assert.IsType<ProjectResponse>(result.Result);
            Assert.True(result.Result.Success);
        }
        [Fact]
        public void Update_Should_Work()
        {
            // Arrange   
            var projectDto = new ProjectDto { Id = 1, Name = "First Update", Description = "Some description for Project1" };
            mockProjRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetSampleProjects()[0]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);

            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var result = projectBL.UpdateAsync(projectDto, It.IsAny<string>());
            // Assert
            Assert.IsType<ProjectResponse>(result.Result);
            Assert.True(result.Result.Success);
            Assert.Equal(projectDto.Name, result.Result.ProjectDTO.Name);
            Assert.Equal(projectDto.Description, result.Result.ProjectDTO.Description);
        }
        [Theory]
        [InlineData("IsObserver")]
        [InlineData("IsDeveloper")]
        [InlineData("IsScrumMaster")]
        [InlineData("IsNone")]
        public async Task Delete_Should_Fail_Only_Owner_Can_Delete(string userIdRole)
        {
            // Arrange   
            mockProjRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetSampleProjects()[0]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember("IsObserver", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember("IsDeveloper", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember("IsScrumMaster", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember("IsNone", It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var result = projectBL.DeleteAsync(It.IsAny<int>(), userIdRole);
            // Assert
            mockProjRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
            await Assert.ThrowsAsync<ForbiddenResponseException>(() => projectBL.DeleteAsync(It.IsAny<int>(), userIdRole));
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(999)]
        public void DeleteAsync_Should_Not_Found(int projectId)
        {         // Arrange   
            string userId = It.IsAny<string>();

            mockProjRepo.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(null as Project);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(userId, projectId)).ReturnsAsync(AppUserRole.Owner);
            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var result = projectBL.DeleteAsync(projectId, userId);
            // Assert
            Assert.IsType<ProjectResponse>(result.Result);
            Assert.False(result.Result.Success);
            Assert.Equal("Project not found", result.Result.Message);
        }
        [Fact]
        public void Delete_Should_Work()
        {
            // Arrange   
            var projectDto = new ProjectDto { Id = 1, Name = "Project for deleting", Description = "Some description for Project1" };
            Project project = mapper.Map<ProjectDto, Project>(projectDto);
            mockProjRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(project);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);

            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var result = projectBL.DeleteAsync(It.IsAny<int>(), It.IsAny<string>());
            // Assert
            Assert.IsType<ProjectResponse>(result.Result);
            Assert.True(result.Result.Success);
            Assert.Equal(projectDto.Name, result.Result.ProjectDTO.Name);
            Assert.Equal(projectDto.Description, result.Result.ProjectDTO.Description);
        }
        [Fact]
        public void Delete_Should_Call_Once_DeleteAsync_Into_Repository()
        {
            // Arrange   
            mockProjRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetSampleProjects()[0]);
            mockProjRepo.Setup(repo => repo.DeleteAsync(It.IsAny<int>()));
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);

            var projectBL = new ProjectBl(mockProjRepo.Object, mapper, mockProjectUserRepo.Object, mockSprintRepo.Object);
            // Act
            var result = projectBL.DeleteAsync(It.IsAny<int>(), It.IsAny<string>());
            // Assert
            mockProjRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Once);
            Assert.IsType<ProjectResponse>(result.Result);
            Assert.True(result.Result.Success);
        }

        public List<Project> GetSampleProjects()
        {
            List<Project> projects = new List<Project>();
            Project project1 = new Project { Id = 1, Name = "First Project", Description = "Some description to Project1" };
            Project project2 = new Project { Id = 2, Name = "Second Project", Description = "Some description to Project2" };

            projects.AddRange(new[] { project1, project2 });
            return projects;
        }
    }
}