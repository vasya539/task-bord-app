using DataAccessLayer.Tests.InMemoryDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories;
using WebApi.Repositories.Interfaces;
using Xunit;

namespace DataAccessLayer.Tests
{
    public class ItemRepositoryTests
    {
        #region GetAll

        [Fact]
        public void GetAllAsyncDefault()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRepository repository = new ItemRepository(context);
                //Act
                List<Item> expected = context.Items.ToList();
                IEnumerable<Item> actual = repository.GetAllAsync().Result;
                //Assert
                Assert.True(actual != null);
                Assert.Equal(expected.Count, actual.ToList().Count);
                Assert.Equal(expected.Count, context.Items.ToList().Count);
                context.Database.EnsureDeleted();
            }
        }

        #endregion GetAll

        #region GetBySprintId

        [Theory]
        [InlineData(2)]
        public async Task GetBySprintIdAsync(int sprintId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRepository repository = new ItemRepository(context);
                //Act
                List<Item> expected = context.Items.Where(r => r.SprintId == sprintId).ToList();
                IEnumerable<Item> actual = await repository.GetBySprintIdAsync(sprintId);
                //Assert
                Assert.True(actual != null);
                Assert.Equal(expected.Count, actual.ToList().Count);
                foreach (var item in actual)
                {
                    Assert.True(item.SprintId == sprintId);
                }
                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData(3)]
        public async Task GetBySprintIdNonCorrectAsync(int sprintId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRepository repository = new ItemRepository(context);
                //Act
                List<Item> expected = context.Items.Where(r => r.SprintId == sprintId).ToList();
                IEnumerable<Item> actual = await repository.GetBySprintIdAsync(sprintId + 1);
                //Assert

                foreach (var item in actual)
                {
                    Assert.False(item.SprintId == sprintId);
                }
                context.Database.EnsureDeleted();
            }
        }

        #endregion GetBySprintId

        #region GetAllChildsAsync

        [Theory]
        [InlineData(1)]
        public async Task GetAllChildsAsync(int parentId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();

            using (var context = cls.GetContextWithData())
            {
                IItemRepository repository = new ItemRepository(context);
                //Act
                List<Item> expected = context.Items.Where(r => r.ParentId == parentId).ToList();
                IEnumerable<Item> actual = await repository.GetAllChildAsync(parentId);
                //Assert
                Assert.True(actual != null);
                Assert.Equal(expected.Count, actual.ToList().Count);
                foreach (var item in actual)
                {
                    Assert.True(item.ParentId == parentId);
                }
                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData(1)]
        public async Task GetAllChildsNotExistingItemAsync(int parentId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();

            using (var context = cls.GetContextWithData())
            {
                IItemRepository repository = new ItemRepository(context);
                //Act
                List<Item> expected = context.Items.Where(r => r.ParentId == parentId).ToList();
                IEnumerable<Item> actual = await repository.GetAllChildAsync(parentId + 10);
                //Assert
                Assert.True(actual != null);
                foreach (var item in actual)
                {
                    Assert.True(item.ParentId != parentId);
                }
                context.Database.EnsureDeleted();
            }
        }

        #endregion GetAllChildsAsync

        #region GetUserStories

        [Theory]
        [InlineData(2)]
        public async Task GetUserStoriesCorrect(int sprintId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();

            using (var context = cls.GetContextWithData())
            {
                IItemRepository repository = new ItemRepository(context);
                //Act
                List<Item> expected = context.Items.Where(r => r.SprintId == sprintId && r.TypeId == 1).ToList();
                IEnumerable<Item> actual = await repository.GetUserStoriesAsync(sprintId);
                //Assert

                foreach (var item in actual)
                {
                    Assert.True(item.TypeId == 1);
                }
                context.Database.EnsureDeleted();
            }
        }

        #endregion GetUserStories

        #region GetUnparentedItems

        [Theory]
        [InlineData(2)]
        public async Task GetUnparentedItemsBySprintId(int sprintId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();

            using (var context = cls.GetContextWithData())
            {
                IItemRepository repository = new ItemRepository(context);
                //Act
                List<Item> expected = context.Items.Where(r => r.SprintId == sprintId && r.ParentId == null).ToList();
                IEnumerable<Item> actual = await repository.GetUnparentedAsync(sprintId);
                //Assert

                foreach (var item in actual)
                {
                    Assert.True(item.ParentId == null);
                }
                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData(1, 3)]
        public async Task GetChildWithSpecificStatusCorrect(int itemId, int statusId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();

            using (var context = cls.GetContextWithData())
            {
                IItemRepository repository = new ItemRepository(context);
                //Act
                List<Item> expected = context.Items.Where(r => r.ParentId == itemId && r.StatusId == statusId && r.IsArchived == false).ToList();
                IEnumerable<Item> actual = await repository.GetChildWithSpecificStatusAsync(itemId, statusId);
                //Assert

                foreach (var item in actual)
                {
                    Assert.True(item.ParentId == itemId && item.StatusId == statusId);
                }
                context.Database.EnsureDeleted();
            }
        }

        #endregion GetUnparentedItems

        #region GetArchivatedBySprintId

        [Theory]
        [InlineData(2)]
        public async Task GetArchivatedBySprintId(int sprintId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();

            using (var context = cls.GetContextWithData())
            {
                IItemRepository repository = new ItemRepository(context);
                //Act
                List<Item> expected = context.Items.Where(r => r.SprintId == sprintId && r.IsArchived == true).ToList();
                IEnumerable<Item> actual = await repository.GetArchivedBySprintIdAsync(sprintId);
                //Assert

                foreach (var item in actual)
                {
                    Assert.True(item.IsArchived == true);
                }
                context.Database.EnsureDeleted();
            }
        }

        #endregion GetArchivatedBySprintId

        #region Read

        [Fact]
        public async Task ReadAsyncExistingItemDefault()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRepository repository = new ItemRepository(context);
                //Act
                Item expected = new Item { Id = 1, SprintId = 1, AssignedUserId = "2138b181-4cee-4b85-9f16-18df308f387d", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = 1, StoryPoint = 2 };
                var actual = await repository.ReadAsync(1);
                //Assert
                Assert.Equal(expected.Id, context.Items.FirstOrDefault(r => r.Id == 1).Id);
                Assert.Equal(expected.Id, actual.Id);
                Assert.IsAssignableFrom<Item>(actual);
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task ReadAsyncNotExistingItemDefault()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRepository repository = new ItemRepository(context);
                //Act
                Item expected = new Item { Id = 1, SprintId = 1, AssignedUserId = "2138b181-4cee-4b85-9f16-18df308f387d", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = 1, StoryPoint = 2 };
                Item actual = await repository.ReadAsync(-2);
                //Assert

                Assert.Null(actual);
                Assert.Null(context.Items.FirstOrDefault(r => r.Id == -2));
                context.Database.EnsureDeleted();
            }
        }

        #endregion Read

        #region Create

        [Fact]
        public async void CreateAsyncDefault()
        {
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetEmptyContextInMemory())
            {
                IItemRepository repository = new ItemRepository(context);

                Item item = new Item { Id = 5, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name5", Description = "Description Item1", StatusId = 1, TypeId = 1 };
                await repository.CreateAsync(item);

                var actual = context.Items.Find(5);

                Assert.NotNull(actual);
                Assert.Equal(item.Id, actual.Id);
                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData(5, 1, "d7f1b614-bf60-4340-9daa-c8dce98fd400", "Item Name5", "Description Item1", 1, 1)]
        public void CreateAsyncWithParameters(int id, int sprintId, string assignedUserId, string name, string description, int statudId, int typeId)
        {
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetEmptyContextInMemory())
            {
                IItemRepository repository = new ItemRepository(context);
                Item item = new Item
                {
                    Id = id,
                    SprintId = sprintId,
                    AssignedUserId = assignedUserId,
                    Name = name,
                    Description = description,
                    StatusId = statudId,
                    TypeId = typeId
                };

                repository.CreateAsync(item);
                var actual = context.Items.Find(id);

                Assert.NotNull(actual);
                Assert.Equal(item.Id, actual.Id);
                context.Database.EnsureDeleted();
            }
        }

        #endregion Create

        #region Update

        [Fact]
        public async Task UpdateAsyncCorrectItemAsync()
        {
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRepository repo = new ItemRepository(context);
                Item item2 = context.Items.FirstOrDefault();
                string oldName = item2.Name;
                item2.Name += "1111";
                await repo.UpdateAsync(item2);
                Item updatedItem = context.Items.Find(item2.Id);

                Assert.NotEqual(updatedItem.Name, oldName);
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task UpdateAsyncNotCorrectItemAsync()
        {
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                IItemRepository repo = new ItemRepository(context);
                Item item2 = context.Items.FirstOrDefault();
                string oldName = item2.Name;
                item2.Id = -2;
                await Assert.ThrowsAsync<InvalidOperationException>(() => repo.UpdateAsync(item2));
                context.Database.EnsureDeleted();
            }
        }

        #endregion Update

        #region Delete

        [Fact]
        public async Task DeleteItemCorrect()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            Item item1 = new Item { Id = 2, SprintId = 1, AssignedUserId = "2138b181-4cee-4b85-9f16-18df308f387d", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = 1, StoryPoint = 2 };
            using (var context = cls.GetEmptyContextInMemory())
            {
                IItemRepository repo = new ItemRepository(context);
                context.Items.Add(item1);
                context.SaveChanges();
                var added = context.Items.Find(item1.Id);

                await repo.DeleteAsync(item1.Id);
                var searched = await repo.ReadAsync(item1.Id);
                //Assert
                Assert.NotNull(added);
                Assert.Null(searched);
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task DeleteItemNotCorrect()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            Item item1 = new Item { Id = 2, SprintId = 1, AssignedUserId = "2138b181-4cee-4b85-9f16-18df308f387d", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = 1, StoryPoint = 2 };
            using (var context = cls.GetEmptyContextInMemory())
            {
                IItemRepository repo = new ItemRepository(context);
                context.Items.Add(item1);
                context.SaveChanges();
                var added = context.Items.Find(item1.Id);

                //Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => repo.DeleteAsync(item1.Id + 1));

                context.Database.EnsureDeleted();
            }
        }

        #endregion Delete
    }
}