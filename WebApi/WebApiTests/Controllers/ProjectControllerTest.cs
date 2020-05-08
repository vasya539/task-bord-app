using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApi.BLs.Communication;
using WebApi.BLs.Interfaces;
using WebApi.Controllers;
using WebApi.Data.DTOs;
using Xunit;

namespace WebApiTests.Controllers
{
    public class ProjectControllerTest
    {
        private readonly Mock<IProjectBl> _mockProjectBL;
        public ProjectControllerTest()
        {
            _mockProjectBL = new Mock<IProjectBl>();
        }
        [Fact]
        public void GetAllAsync_Should_Work()
        {
            // Arrange
            _mockProjectBL.Setup(repo => repo.GetAllAsync()).ReturnsAsync(GetSampleProjects());
            var projectController = new ProjectController(_mockProjectBL.Object);
            // Act
            var res1 = projectController.GetAllAsync().Result;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(viewResult.Value);
            _mockProjectBL.Verify(r => r.GetAllAsync(), Times.Once);
            Assert.Equal(GetSampleProjects().Count(), model.Count());
        }
        [Fact]
        public void GetAllAsync_Should_Return_NotFoundResult()
        {
            // Arrange
            _mockProjectBL.Setup(repo => repo.GetAllAsync()).ReturnsAsync(null as IEnumerable<ProjectDto>);
            var projectController = new ProjectController(_mockProjectBL.Object);
            // Act
            var res1 = projectController.GetAllAsync().Result;
            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(res1);
            var model = Assert.IsAssignableFrom<NotFoundResult>(viewResult);
            Assert.Equal(404, model.StatusCode);
        }
        [Fact]
        public void GetProject_Should_Work()
        {
            // Arrange
            _mockProjectBL.Setup(repo => repo.GetByIdAsync(1, It.IsAny<string>())).ReturnsAsync(GetSampleProjects().FirstOrDefault());
            var projectController = new ProjectController(_mockProjectBL.Object);
            // Act
            var res1 = projectController.GetProject(1).Result;
            var exepted = GetSampleProjects().FirstOrDefault();
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<ProjectDto>(viewResult.Value);
            _mockProjectBL.Verify(r => r.GetByIdAsync(1, It.IsAny<string>()), Times.Once);
            Assert.Equal(exepted.Id, model.Id);
            Assert.Equal(exepted.Name, model.Name);
            Assert.Equal(exepted.Description, model.Description);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(999)]
        public void GetProject_Should_Return_NotFoundResult(int id)
        {
            // Arrange
            _mockProjectBL.Setup(repo => repo.GetByIdAsync(id, It.IsAny<string>())).ReturnsAsync(null as ProjectDto);
            var projectController = new ProjectController(_mockProjectBL.Object);
            // Act
            var res1 = projectController.GetProject(1).Result;
            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(res1);
            var model = Assert.IsAssignableFrom<NotFoundResult>(viewResult);
            Assert.Equal(404, model.StatusCode);
        }
        [Fact]
        public void UpdateProject_Should_Work_Return_Ok()
        {
            // Arrange
            ProjectDto projectDto = new ProjectDto { Id = 1, Name = "Project name", Description = "Some description" };
            ProjectResponse response = new ProjectResponse(projectDto);
            _mockProjectBL.Setup(repo => repo.UpdateAsync(projectDto, It.IsAny<string>())).ReturnsAsync(response);
            var projectController = new ProjectController(_mockProjectBL.Object);
            // Act
            var res1 = projectController.UpdateProject(projectDto).Result;
            // Assert
            var viewResult = Assert.IsType<OkResult>(res1);
            _mockProjectBL.Verify(r => r.UpdateAsync(projectDto, It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public void UpdateProject_Should_Return_BadRequest()
        {
            // Arrange          
            ProjectResponse response = new ProjectResponse("message");
            _mockProjectBL.Setup(repo => repo.UpdateAsync(It.IsAny<ProjectDto>(), It.IsAny<string>())).ReturnsAsync(response);
            var projectController = new ProjectController(_mockProjectBL.Object);
            // Act
            var res1 = projectController.UpdateProject(It.IsAny<ProjectDto>()).Result;
            // Assert
            var viewResult = Assert.IsType<BadRequestObjectResult>(res1);
            var message= Assert.IsAssignableFrom<string>(viewResult.Value);
            Assert.Equal("message", message);
        }
        [Fact]
        public void CreateProject_Should_Work_Return_Ok()
        {
            // Arrange
            ProjectDto projectDto = new ProjectDto { Id = 1, Name = "Project name", Description = "Some description" };
            ProjectResponse response = new ProjectResponse(projectDto);
            _mockProjectBL.Setup(repo => repo.CreateAsync(projectDto, It.IsAny<string>())).ReturnsAsync(response);
            var projectController = new ProjectController(_mockProjectBL.Object);
            // Act
            var res1 = projectController.CreateProject(projectDto).Result;
            // Assert
            var viewResult = Assert.IsType<OkResult>(res1);
            _mockProjectBL.Verify(r => r.CreateAsync(projectDto, It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public void CreateProject_Should_Return_BadRequest()
        {
            // Arrange          
            ProjectResponse response = new ProjectResponse("message");
            _mockProjectBL.Setup(repo => repo.CreateAsync(It.IsAny<ProjectDto>(), It.IsAny<string>())).ReturnsAsync(response);
            var projectController = new ProjectController(_mockProjectBL.Object);
            // Act
            var res1 = projectController.CreateProject(It.IsAny<ProjectDto>()).Result;
            // Assert
            var viewResult = Assert.IsType<BadRequestObjectResult>(res1);
            var message = Assert.IsAssignableFrom<string>(viewResult.Value);
            Assert.Equal("message", message);
        }
        [Fact]
        public void DeleteProject_Should_Work_Return_Ok()
        {
            // Arrange
            ProjectDto projectDto = new ProjectDto { Id = 1, Name = "Project name", Description = "Some description" };
            ProjectResponse response = new ProjectResponse(projectDto);
            _mockProjectBL.Setup(repo => repo.DeleteAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(response);
            var projectController = new ProjectController(_mockProjectBL.Object);
            // Act
            var res1 = projectController.DeleteProject(It.IsAny<int>()).Result;
            // Assert
            var viewResult = Assert.IsType<OkResult>(res1);
            _mockProjectBL.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public void DeleteProject_Should_Return_BadRequest()
        {
            // Arrange          
            ProjectResponse response = new ProjectResponse("message");
            _mockProjectBL.Setup(repo => repo.DeleteAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(response);
            var projectController = new ProjectController(_mockProjectBL.Object);
            // Act
            var res1 = projectController.DeleteProject(It.IsAny<int>()).Result;
            // Assert
            var viewResult = Assert.IsType<BadRequestObjectResult>(res1);
            var message = Assert.IsAssignableFrom<string>(viewResult.Value);
            Assert.Equal("message", message);
        }

        public IEnumerable<ProjectDto> GetSampleProjects()
        {
            List<ProjectDto> projects = new List<ProjectDto>
            {
                new ProjectDto { Id = 1, Name = "First Project", Description = "Some description to Project1" },
                new ProjectDto { Id = 2, Name = "Second Project", Description = "Some description to Project2" }
            };
            return projects as IEnumerable<ProjectDto>;
        }

    }

}
