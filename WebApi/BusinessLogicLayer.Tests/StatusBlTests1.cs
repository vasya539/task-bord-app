
using Microsoft.EntityFrameworkCore;
using System;
using WebApi.Data;
using Xunit;
using System.Threading.Tasks;
using WebApi.Data.Models;
using System.Collections.Generic;
using System.Linq;
using WebApi.Repositories.Interfaces;
using WebApi.BLs;
using WebApi.BLs.Interfaces;
using WebApi.Repositories;
using DataAccessLayer.Tests.InMemoryDatabase;
using AutoMapper;
using Moq;
using WebApi.Data.DTOs;

namespace BusinessLogicLayer.Tests
{
    public class StatusBlTests
    {
        private readonly Mock<IStatusRepository> mockRepo;
        private readonly Mock<IMapper> mockMapper;

        public StatusBlTests()
        {
            mockRepo = new Mock<IStatusRepository>();
            mockMapper = new Mock<IMapper>();
        }

        [Fact]
        public void GetAllAsync_Should_Returns_AllStatuses()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(GetSampleStatuses());
            var statusBL = new StatusBl(mockRepo.Object, mockMapper.Object);
            // Act
            var result = statusBL.GetAllAsync();
            // Assert
            var typeResult = Assert.IsType<Task<List<StatusDto>>>(result);
            var model = Assert.IsAssignableFrom<List<StatusDto>>(typeResult.Result);
            Assert.Equal(GetSampleStatuses().Count, model.Count());
        }
        [Fact]
        public void Read_Should_Call_Once_ReadAsync()
        {
            int id = 1;
            // Arrange
            var statusBL = new StatusBl(mockRepo.Object, mockMapper.Object);
            // Act
            var result = statusBL.Read(id);
            // Assert
            var typeResult = Assert.IsType<Task<StatusDto>>(result);
            mockRepo.Verify(r => r.ReadAsync(id), Times.Once);
        }

        [Fact]
        public void Create_Should_Call_Once_CreateAsync()
        {
            // Arrange
            var statusBL = new StatusBl(mockRepo.Object, mockMapper.Object);
            // Act
            var status1 = new StatusDto { Id = 1, Name = "New" };
            var res = statusBL.Create(status1);
            // Assert
            mockRepo.Verify(r => r.CreateAsync(null), Times.Once());
        }
        [Fact]
        public void Update_Should_Call_Once_CreateAsync()
        {
            // Arrange
            var statusBL = new StatusBl(mockRepo.Object, mockMapper.Object);
            // Act
            var status1 = new StatusDto { Id = 1, Name = "New" };
            var res = statusBL.Update(status1);
            // Assert
            mockRepo.Verify(r => r.UpdateAsync(null), Times.Once());
        }
        [Fact]
        public void Create_Should_Call_Once_DeleteAsync()
        {
            // Arrange
            var statusBL = new StatusBl(mockRepo.Object, mockMapper.Object);
            // Act
            var result = statusBL.Delete(1);
            // Assert
            mockRepo.Verify(r => r.DeleteAsync(1), Times.Once());
        }
        public List<Status> GetSampleStatuses()
        {
            List<Status> statuses = new List<Status>();
            Status status1 = new Status { Id = 1, Name = "New" };
            Status status2 = new Status { Id = 2, Name = "Approved" };
            Status status3 = new Status { Id = 3, Name = "Done" };
            statuses.AddRange(new[] { status1, status2, status3 });
            return statuses;
        }
    }
}
