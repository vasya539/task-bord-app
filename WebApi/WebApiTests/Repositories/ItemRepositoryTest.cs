using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Models;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories;
using WebApiTests.TestingResources;
using Xunit;

namespace WebApiTests.Repositories
{
    public class ItemRepositoryTest
    {
        public int Id = 100;

        #region GetAllItem

        [Fact]
        public async Task GetAllItems_ReturnAsync()
        {
            //Arrange
            var option = TestDbContext.GetNewOptions();

            using (var context = new AppDbContext(option))
            {
                context.Items.AddRange(GetTestItems());
                context.SaveChanges();
            }

            //Act
            using (var context = new AppDbContext(option))
            {
                var mockRepo = new Mock<ItemRepository>(context);
                var result = await mockRepo.Object.GetAllAsync();

                //Assert
                Assert.NotEmpty(result);
                Assert.NotNull(result);
                Assert.IsAssignableFrom<IEnumerable<Item>>(result);
            }
        }

        #endregion GetAllItem

        #region GetByID

        [Fact]
        public async Task GetItemById_ReturnAsync()
        {
            //Arrange

            var option = TestDbContext.GetNewOptions();

            using (var context = new AppDbContext(option))
            {
                context.Items.Add(new Item { Id = Id, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Description = "1", Name = "1", SprintId = 1, StatusId = 1, TypeId = 1 });
                context.SaveChanges();
            }

            //Act
            using (var context = new AppDbContext(option))
            {
                var mockRepo = new Mock<ItemRepository>(context);
                IItemRepository realRepo = new ItemRepository(context);
                var result = await mockRepo.Object.ReadAsync(Id);

                Assert.NotNull(result);
                Assert.IsAssignableFrom<Item>(result);
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task GetNotExistedItemById_ReturnAsync()
        {
            //Arrange

            var option = TestDbContext.GetNewOptions();

            using (var context = new AppDbContext(option))
            {
                context.Items.Add(new Item { Id = Id, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Description = "1", Name = "1", SprintId = 1, StatusId = 1, TypeId = 1 });
                context.SaveChanges();
            }

            //Act
            using (var context = new AppDbContext(option))
            {
                var mockRepo = new Mock<ItemRepository>(context);

                var result = await mockRepo.Object.ReadAsync(Id + 100);

                //Assert
                Assert.Null(result);
                context.Database.EnsureDeleted();
            }
        }

        #endregion GetByID

        #region CreateItem

        [Fact]
        public async Task CreateItemRight()
        {
            //Arrange

            var option = TestDbContext.GetNewOptions();

            using (var context = new AppDbContext(option))
            {
                var mockRepo = new Mock<ItemRepository>(context);

                await mockRepo.Object.CreateAsync(new Item { Id = Id, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Description = "1", Name = "1", SprintId = 1, StatusId = 1, TypeId = 1 });
            }

            //Act
            using (var context = new AppDbContext(option))
            {
                var result = context.Items.Find(Id);

                //Assert
                Assert.NotNull(result);
                Assert.IsType<Item>(result);
                Assert.IsAssignableFrom<Item>(result);
                context.Database.EnsureDeleted();
            }
        }

        #endregion CreateItem

        #region UpdateItem

        [Fact]
        public async Task UpdateItemRight()
        {
            //Arrange

            var option = TestDbContext.GetNewOptions();
            var firstItem = new Item { Id = Id, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Description = "title", Name = "1", SprintId = 1, StatusId = 1, TypeId = 1 };

            using (var context = new AppDbContext(option))
            {
                context.Items.Add(firstItem);
                context.SaveChanges();
            }
            var secondItem = new Item { Id = Id, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Description = "New title", Name = "1", SprintId = 1, StatusId = 1, TypeId = 1 };

            //Act
            using (var context = new AppDbContext(option))
            {
                IItemRepository repo = new ItemRepository(context);
                var mockRepo = new Mock<ItemRepository>(context);

                await repo.UpdateAsync(secondItem);

                //Assert
                Assert.NotEqual(firstItem.Description, context.Items.Find(Id).Description);
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task UpdateItemWithBadParameters()
        {
            //Arrange

            var option = TestDbContext.GetNewOptions();
            var firstItem = new Item { Id = Id, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Description = "title", Name = "1", SprintId = 1, StatusId = 1, TypeId = 1 };

            using (var context = new AppDbContext(option))
            {
                context.Items.Add(firstItem);
                context.SaveChanges();

                var secondItem = new Item { Id = Id + 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Description = "New title", Name = "1", SprintId = 1, StatusId = 1, TypeId = 1 };

                IItemRepository repo = new ItemRepository(context);
                var mockRepo = new Mock<ItemRepository>(context);

                //Act-Assert
                await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => repo.UpdateAsync(secondItem));
                context.Database.EnsureDeleted();
            }
        }

        #endregion UpdateItem

        #region DeleteItem

        [Fact]
        public async Task DeleteItemRight()
        {
            //Arrange

            var option = TestDbContext.GetNewOptions();
            var firstItem = new Item { Id = Id, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Description = "title", Name = "1", SprintId = 1, StatusId = 1, TypeId = 1 };
            int startCount;
            Item searched;

            //Act
            using (var context = new AppDbContext(option))
            {
                context.Items.Add(firstItem);
                context.SaveChanges();
                var itemCount = context.Items.CountAsync();
                startCount = itemCount.Result;

                IItemRepository repo = new ItemRepository(context);
                await repo.DeleteAsync(firstItem.Id);
                searched = await repo.ReadAsync(firstItem.Id);
                context.Database.EnsureDeleted();
            }

            //Assert
            Assert.Null(searched);
        }

        [Fact]
        public async Task DeleteItemWithBadProperties()
        {
            //Arrange
            var option = TestDbContext.GetNewOptions();
            var firstItem = new Item { Id = Id, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Description = "title", Name = "1", SprintId = 1, StatusId = 1, TypeId = 1 };
            int startCount;

            using (var context = new AppDbContext(option))
            {
                context.Items.Add(firstItem);
                context.SaveChanges();
                var itemCount = context.Items.CountAsync();
                startCount = itemCount.Result;
                firstItem.Id++;

                IItemRepository repo = new ItemRepository(context);

                //Act-Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => repo.DeleteAsync(firstItem.Id));
                context.Database.EnsureDeleted();
            }
        }

        #endregion DeleteItem

        private List<Item> GetTestItems()
        {
            var items = new List<Item>
            {
                new Item { Id=1, AssignedUserId="d7f1b614-bf60-4340-9daa-c8dce98fd400", Description="Desc1", Name="TestItem1", SprintId=1, StatusId=1, TypeId=1},
                new Item { Id=2, AssignedUserId="d7f1b614-bf60-4340-9daa-c8dce98fd400", Description="Desc2", Name="TestItem2", SprintId=1, StatusId=1, TypeId=1},
                new Item { Id=3, AssignedUserId="d7f1b614-bf60-4340-9daa-c8dce98fd400", Description="Desc3", Name="TestItem3", SprintId=1, StatusId=1, TypeId=1},
                new Item { Id=4, AssignedUserId="d7f1b614-bf60-4340-9daa-c8dce98fd400", Description="Desc4", Name="TestItem4", SprintId=1, StatusId=1, TypeId=1,}
            };
            return items;
        }
    }
}