using DataAccessLayer.Tests.InMemoryDatabase;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Data.Models;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories;
using Xunit;
using System.Linq;
using WebApi.Repositories.Interfaces;
using System;

namespace DataAccessLayer.Tests
{
    public class SprintRepositoryTests
    {
        [Fact]
        public void GetAllByProjectIdAsync_Sprint_ShouldWork_WhithTheSameContext()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            var context = cls.GetContextWithData();
            try
            {
                ISprintRepository repository = new SprintRepository(context);
                //Act
                List<Sprint> expected = context.Sprints.ToList();
                IEnumerable<Sprint> actual = repository.GetAllByProjectIdAsync(1).Result;
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
        public void GetByIdAsync_Sprint_ShouldWork_WhithCorrectId()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            var context = cls.GetContextWithData();
            try
            {
                ISprintRepository repository = new SprintRepository(context);
                //Act
                Sprint expected = context.Sprints.Find(1);
                Sprint actual = repository.GetByIdAsync(1).Result;
                Sprint expected2 = context.Sprints.Find(2);
                Sprint actual2 = repository.GetByIdAsync(2).Result;
                //Assert
                Assert.Equal(expected, actual);
                Assert.Equal(expected2, actual2);

                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.ProjectId, actual.ProjectId);
                Assert.Equal(expected.StartDate, actual.StartDate);
            }
            finally
            {
                context.Database.EnsureDeleted();
                context.Dispose();
            }

        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(9999)]
        public void GetByIdAsync_Sprint_ShouldReturnNull_WithIncorrectID(int id)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            var context = cls.GetContextWithData();
            try
            {
                ISprintRepository repository = new SprintRepository(context);
                //Act
                Sprint actual = repository.GetByIdAsync(id).Result;
                //Assert
                Assert.Null(actual);
            }
            finally
            {
                context.Database.EnsureDeleted();
                context.Dispose();
            }
        }
        [Theory]
        [InlineData(5, 1)]
        [InlineData(6, 2)]
        [InlineData(7, 2)]
        public void CreateAsync_ShouldWork(int id, int projectId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            var context = cls.GetContextWithData();
            try
            {
                ISprintRepository repository = new SprintRepository(context);
                //Act
                Sprint sprint = new Sprint { Id = id, ProjectId = projectId };
                repository.CreateAsync(sprint);
                var actual = context.Sprints.Find(id);
                //Assert
                Assert.NotNull(actual);
                Assert.Equal(sprint.Id, actual.Id);
                Assert.Equal(sprint.ProjectId, actual.ProjectId);
            }
            finally
            {
                context.Database.EnsureDeleted();
                context.Dispose();
            }
        }

        [Theory]
        [InlineData(5, 5)]
        [InlineData(6, 5)]
        [InlineData(7, 5)]
        public void CreateAsync_ShouldCreateManySprints(int id, int projectId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            var context = cls.GetContextWithData();
            try
            {
                ISprintRepository repository = new SprintRepository(context);
                Sprint sprint = new Sprint { Id = id, ProjectId = projectId, StartDate = new DateTime(2020, 4, 23), EndDate = new DateTime(2020, 5, 23) };
                //Act
                repository.CreateAsync(sprint);
                var actual = context.Sprints.Find(id);
                //Assert
                Assert.Equal(sprint.Id, actual.Id);
                Assert.Equal(sprint.ProjectId, actual.ProjectId);
                Assert.Equal(sprint.StartDate, actual.StartDate);
                Assert.Equal(sprint.EndDate, actual.EndDate);
            }
            finally
            {
                context.Database.EnsureDeleted();
                context.Dispose();
            }
        }
        [Fact]
        public void UpdateAsync_ShouldUpdateSprint()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            var context = cls.GetContextWithData();
            try
            {
                ISprintRepository repository = new SprintRepository(context);
                Sprint sprint = context.Sprints.Find(1);
                sprint.ProjectId = 2;
                //Act
                repository.UpdateAsync(sprint);
                var actual = context.Sprints.Find(1);
                //Assert
                Assert.Equal(sprint.Id, actual.Id);
                Assert.NotEqual(1, actual.ProjectId);
                Assert.Equal(sprint.ProjectId, actual.ProjectId);
            }
            finally
            {
                context.Database.EnsureDeleted();
                context.Dispose();
            }
        }
    }
}