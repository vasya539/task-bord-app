using DataAccessLayer.Tests.InMemoryDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Data.Models;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories;
using Xunit;

namespace DataAccessLayer.Tests
{
    public class ProjectRepositoryTests
    {
        [Fact]
        public void GetAllAsync_Should_Returns_AllProjects()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            var context = cls.GetContextWithData();
            try
            {
                
                IProjectRepository repository = new ProjectRepository(context);
                //Act
                List<Project> expected = context.Projects.ToList();
                IEnumerable<Project> actual = repository.GetAllAsync().Result;
                //Assert
                Assert.True(actual != null);
                Assert.Equal(expected.Count, actual.ToList().Count);
                Assert.Equal(expected, actual);          
            }
            finally
            {
                context.Database.EnsureDeleted();
                context.Dispose();
            }
        }

        [Fact]
        public void GetByIdAsync_Should_Work()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            var context = cls.GetContextWithData();

            try
            {
                IProjectRepository repository = new ProjectRepository(context);
                //Act
                Project expected = context.Projects.Find(1);
                Project actual = repository.GetByIdAsync(1).Result;
                //Assert
                Assert.NotNull(actual);
                Assert.Equal(expected, actual);
            }
            finally
            {
                context.Database.EnsureDeleted();
                context.Dispose();
            }
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-11)]
        [InlineData(999)]
        public void GetByIdAsync_Should_Fail_Whith_IncorrectId(int incorrectId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            var context = cls.GetContextWithData();

            try
            {
                IProjectRepository repository = new ProjectRepository(context);
                //Act
                Project expected = context.Projects.Find(incorrectId);
                Project actual = repository.GetByIdAsync(incorrectId).Result;
                //Assert
                Assert.Null(actual);
                Assert.Equal(expected, actual);
            }
            finally
            {
                context.Database.EnsureDeleted();
                context.Dispose();
            }
        }

        [Theory]
        [InlineData("Project 1", "Description for project 1")]
        [InlineData("Project 2", "Description for project 2")]
        public void CreateAsync_ShouldWork(string name, string description)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            var context = cls.GetContextWithData();

            try
            {
                IProjectRepository repository = new ProjectRepository(context);
                Project newProject = new Project { Name = name, Description = description };
                //Act
                repository.CreateAsync(newProject);
                Project actual = context.Projects.Find(newProject.Id);

                //Assert
                Assert.NotNull(actual);
                Assert.Equal(newProject.Name, actual.Name);
                Assert.Equal(newProject.Description, actual.Description);
            }
            finally
            {
                context.Database.EnsureDeleted();
                context.Dispose();
            }
        }
        [Fact]
        public void UpdateAsync_Should_Work()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            int id = 1;
            var context = cls.GetContextWithData();

            try
            {
                context.Database.EnsureDeleted();
                IProjectRepository repository = new ProjectRepository(context);
                Project project = context.Projects.Find(id);
                //Act
                project.Name = "Updated name";
                project.Description = "Updated description";

                repository.UpdateAsync(project);
                Project actual = context.Projects.Find(id);

                //Assert

                    Assert.NotNull(actual);
                    Assert.Equal("Updated name", actual.Name);
                    Assert.Equal("Updated description", actual.Description);
            }
            finally
            {
                context.Database.EnsureDeleted();
                context.Dispose();
            }
        }
        [Fact]
        public void DeleteAsync_Should_Work()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            int id = 1;
            var context = cls.GetContextWithData();

            try
            {
                IProjectRepository repository = new ProjectRepository(context);

                //Act
                repository.DeleteAsync(id);
                Project actual = context.Projects.Find(id);

                // Assert
                Assert.Null(actual);


            }
            finally
            {
                context.Database.EnsureDeleted();
                context.Dispose();
            }
        }
    }
}
