using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Tests.InMemoryDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Data.Models;
using WebApi.Repositories;
using WebApi.Repositories.Interfaces;
using Xunit;

namespace DataAccessLayer.Tests
{
    public class ItemRelationRepositoryTests
    {
        [Theory]
        [InlineData(1, 2)]
        public async Task Check_GetRecordReturnOneRelation(int firstId, int secondId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRelationRepository repo = new ItemRelationRepository(context);
                ItemRelation expectedRelation = context.ItemsRelations.Find(firstId, secondId);
                ItemRelation actualRelation = await repo.GetRecordAsync(firstId, secondId);

                Assert.True(actualRelation != null);
                Assert.Equal(expectedRelation, actualRelation);
                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData(100, 2)]
        public async Task Check_GetNotExistRecordReturnsNull(int firstId, int secondId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRelationRepository repo = new ItemRelationRepository(context);
                ItemRelation expectedRelation = context.ItemsRelations.Find(firstId, secondId);
                ItemRelation actualRelation = await repo.GetRecordAsync(firstId, secondId);

                Assert.Null(actualRelation);
                Assert.Equal(expectedRelation, actualRelation);
                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData(1, 6)]
        public async Task Check_CreateRelationWorkingRight(int firstId, int secondId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRelationRepository repo = new ItemRelationRepository(context);
                ItemRelation startRelation = new ItemRelation() { FirstItemId = firstId, SecondItemId = secondId };

                await repo.CreateRecordAsync(startRelation);
                var actualRelation = context.ItemsRelations.Find(firstId, secondId);

                Assert.NotNull(actualRelation);
                Assert.Equal(startRelation, actualRelation);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData(1, 2)]
        public async Task Check_DeleteRelationWorkingRight(int firstId, int secondId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRelationRepository repo = new ItemRelationRepository(context);
                ItemRelation startRelation = context.ItemsRelations.Find(firstId, secondId);

                await repo.DeleteRecordAsync(startRelation);
                var actualRelation = context.ItemsRelations.Find(firstId, secondId);

                Assert.NotNull(startRelation);
                Assert.Null(actualRelation);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData(1, 2)]
        public async Task Check_DeleteNotExistRelationThrowsException(int firstId, int secondId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRelationRepository repo = new ItemRelationRepository(context);
                ItemRelation startRelation = context.ItemsRelations.Find(firstId, secondId);
                startRelation.FirstItemId -= 100;
                await Assert.ThrowsAsync<InvalidOperationException>(() => repo.DeleteRecordAsync(startRelation));
                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData(1)]
        public async Task Check_GetRelatedItemsWorkingRight(int itemId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRelationRepository repo = new ItemRelationRepository(context);
                var expected = await context.ItemsRelations
                    .Where(r => r.FirstItemId == itemId || r.SecondItemId == itemId)
                    .ToListAsync();

                var actual = await repo.GetRelatedItems(itemId);

                Assert.NotEmpty(actual);
                Assert.Equal(expected, actual);
                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData(100)]
        public async Task Check_GetRelatedItemsReturnEmptyCollection(int itemId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRelationRepository repo = new ItemRelationRepository(context);
                var expected = await context.ItemsRelations
                    .Where(r => r.FirstItemId == itemId || r.SecondItemId == itemId)
                    .ToListAsync();

                var actual = await repo.GetRelatedItems(itemId);

                Assert.Empty(actual);
                Assert.Equal(expected, actual);
                context.Database.EnsureDeleted();
            }
        }
    }
}