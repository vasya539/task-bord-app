using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using WebApi.Data;
using WebApi.Data.Models;

namespace DataAccessLayer.Tests.InMemoryDatabase
{
    public class InMemoryAppDbContext
    {
        private static int _uniqueDbNumber = 0;
        private static InMemoryAppDbContext _instance = new InMemoryAppDbContext();


        public static AppDbContext GetEmptyUniqueAppDbContext()
        {
            int numb = System.Threading.Interlocked.Increment(ref _uniqueDbNumber);

            var opts =
                new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"UniqueInMemDb_{numb}")
                .Options;

            return new AppDbContext(opts);
        }

        public static AppDbContext GetUniqueAppDbContext()
        {
            int numb = System.Threading.Interlocked.Increment(ref _uniqueDbNumber);

            var opts =
                new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"UniqueInMemDb_{numb}")
                .Options;

            return _instance.InsertDataToDB(new AppDbContext(opts));
        }

        public AppDbContext GetContextWithData()
        {
            return InsertDataToDB(GetEmptyContextInMemory());
        }

        public AppDbContext GetEmptyContextInMemory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"DbInMemory")
                .Options;

            return new AppDbContext(options);
        }

        internal AppDbContext InsertDataToDB(AppDbContext context)
        {
            Project project1 = new Project { Id = 1, Name = "First Project", Description = "Some description to Project1" };
            Project project2 = new Project { Id = 2, Name = "Second Project", Description = "Some description to Project2" };

            Sprint sprint1 = new Sprint { Id = 1, ProjectId = 1, StartDate = DateTime.Today, Name = "Sprint 1", Description = "Sprint descr 1", EndDate = DateTime.Today.AddDays(7) };
            Sprint sprint2 = new Sprint { Id = 2, ProjectId = 1, StartDate = DateTime.Today, Name = "Sprint 1", Description = "Sprint descr 1", EndDate = DateTime.Today.AddDays(7) };

            Item item1 = new Item { Id = 1, SprintId = 1, AssignedUserId = "2138b181-4cee-4b85-9f16-18df308f387d", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = null, StoryPoint = 2 };
            Item item2 = new Item { Id = 2, SprintId = 1, AssignedUserId = "2514591e-29f0-4a63-b0ad-87a3e7ebec3d", Name = "Item Name2", Description = "Description Item2", StatusId = 2, TypeId = 2, IsArchived = false, ParentId = 1, StoryPoint = 2 };
            Item item3 = new Item { Id = 3, SprintId = 2, AssignedUserId = "421cb65f-a76d-4a73-8a1a-d792f37ef992", Name = "Item Name3", Description = "Description Item3", StatusId = 3, TypeId = 1, IsArchived = true, ParentId = 1, StoryPoint = 2 };
            Item item4 = new Item { Id = 4, SprintId = 2, AssignedUserId = "54bfd1f9-d379-4930-9c3b-4c84992c028e", Name = "Item Name4", Description = "Description Item4", StatusId = 3, TypeId = 1, IsArchived = false, ParentId = 2, StoryPoint = 2 };
            Item item5 = new Item { Id = 5, SprintId = 2, AssignedUserId = "54bfd1f9-d379-4930-9c3b-4c84992c028e", Name = "Item Name5", Description = "Description Item5", StatusId = 3, TypeId = 1, IsArchived = true, ParentId = null, StoryPoint = 2 };

            Comment comment1 = new Comment { Id = 1, ItemId = 1, Text = "Comment text1", UserId = "2138b181-4cee-4b85-9f16-18df308f387d", Date = DateTime.Today };
            Comment comment2 = new Comment { Id = 2, ItemId = 2, Text = "Comment text2", UserId = "421cb65f-a76d-4a73-8a1a-d792f37ef992", Date = DateTime.Today };

            Status status1 = new Status { Id = 1, Name = "New" };
            Status status2 = new Status { Id = 2, Name = "Approved" };
            Status status3 = new Status { Id = 3, Name = "Done" };

            ItemType itemType1 = new ItemType { Id = 1, Name = "UserStory" };
            ItemType itemType2 = new ItemType { Id = 2, Name = "Task" };
            ItemType itemType3 = new ItemType { Id = 3, Name = "Bug" };
            ItemType itemType4 = new ItemType { Id = 4, Name = "Test" };

            User user1 = new User { Id = "2138b181-4cee-4b85-9f16-18df308f387d", UserName = "MyLogin1", NormalizedUserName = "MYLOGIN1", PasswordHash = "MyPass", FirstName = "Bart", LastName = "Simpson", Email = "bart@simpson.com", Info = "in-memory user" };
            User user2 = new User { Id = "2514591e-29f0-4a63-b0ad-87a3e7ebec3d", UserName = "MyLogin2", NormalizedUserName = "MYLOGIN2", PasswordHash = "MyPass", FirstName = "Lisa", LastName = "Simpson", Email = "lisa@simpson.com", Info = "in-memory user" };
            User user3 = new User { Id = "421cb65f-a76d-4a73-8a1a-d792f37ef992", UserName = "MyLogin3", NormalizedUserName = "MYLOGIN3", PasswordHash = "MyPass", FirstName = "Homer", LastName = "Simpson", Email = "homer@simpson.com", Info = "in-memory user" };
            User user4 = new User { Id = "54bfd1f9-d379-4930-9c3b-4c84992c028e", UserName = "MyLogin4", NormalizedUserName = "MYLOGIN4", PasswordHash = "MyPass", FirstName = "Marge", LastName = "Simpson", Email = "marge@simpson.com", Info = "in-memory user" };

            AppUserRole appUserRole1 = new AppUserRole { Id = 1, Name = "NoMember" };
            AppUserRole appUserRole2 = new AppUserRole { Id = 2, Name = "Owner" };
            AppUserRole appUserRole3 = new AppUserRole { Id = 3, Name = "ScrumMaster" };
            AppUserRole appUserRole4 = new AppUserRole { Id = 4, Name = "Developer" };
            AppUserRole appUserRole5 = new AppUserRole { Id = 5, Name = "Observer" };

            ItemRelation relation1 = new ItemRelation { FirstItemId = 1, SecondItemId = 2 };
            ItemRelation relation2 = new ItemRelation { FirstItemId = 1, SecondItemId = 3 };
            ItemRelation relation3 = new ItemRelation { FirstItemId = 1, SecondItemId = 4 };
            ItemRelation relation4 = new ItemRelation { FirstItemId = 4, SecondItemId = 7 };
            ItemRelation relation5 = new ItemRelation { FirstItemId = 4, SecondItemId = 8 };
            ItemRelation relation6 = new ItemRelation { FirstItemId = 6, SecondItemId = 2 };
            ItemRelation relation7 = new ItemRelation { FirstItemId = 2, SecondItemId = 9 };

            context.AppUserRoles.AddRange(new[]
            {
                AppUserRole.None,
                AppUserRole.Owner,
                AppUserRole.ScrumMaster,
                AppUserRole.Developer,
                AppUserRole.Observer
            });
            context.Comments.AddRange(new[] { comment1, comment2 });
            context.Projects.AddRange(new[] { project1, project2 });
            context.Sprints.AddRange(new[] { sprint1, sprint2 });
            context.Items.AddRange(new[] { item1, item2, item3, item4 });
            context.Statuses.AddRange(new[] { status1, status2, status3 });
            context.ItemTypes.AddRange(new[] { itemType1, itemType2 });
            context.Users.AddRange(new[] { user1, user2, user3, user4 });
            context.ItemsRelations.AddRange(new[] { relation1, relation2, relation3, relation4, relation5, relation6, relation7 });

            context.SaveChanges();
            return context;
        }
    }
}