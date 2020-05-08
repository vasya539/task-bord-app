using DataAccessLayer.Tests.InMemoryDatabase;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WebApi.BLs.Interfaces;
using WebApi.Data;
using WebApi.Repositories;
using WebApi.Repositories.Interfaces;
using Xunit;

namespace DataAccessLayer.Tests
{
    
    public class StatusRepositoryTests
    {

        [Fact]
        public void SetUp()
        {
            var statusRepository = new Mock<IStatusRepository>();
            var statusService = new Mock<IStatusBl>();
        }
    }
}
