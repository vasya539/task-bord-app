using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Data.Models;
using System;
using System.Runtime.CompilerServices;


namespace WebApiTests.TestingResources
{
    public class TestDbContext 
    {
        public static DbContextOptions<AppDbContext> GetNewOptions(string name = "")
        {
            string DbName = "TestDb" + name;

            return new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(DbName).Options;

        }
    }
}