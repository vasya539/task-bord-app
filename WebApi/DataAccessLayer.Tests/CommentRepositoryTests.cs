using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Tests.InMemoryDatabase;
using Microsoft.EntityFrameworkCore;
using WebApi.Data.Models;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories;
using WebApi.Repositories.Interfaces;
using Xunit;

namespace DataAccessLayer.Tests
{
    public class CommentRepositoryTests
    {
        [Fact]
        public async Task GetAllAsyncReturnsCorrect()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                ICommentRepository repository = new CommentRepository(context);
                //Act
                List<Comment> expected = context.Comments.ToList();
                IEnumerable<Comment> actual = await repository.GetAllAsync();
                //Assert
                Assert.True(actual != null);
                Assert.Equal(expected.Count, actual.ToList().Count);
                Assert.Equal(expected.Count, context.Comments.ToList().Count);
                context.Database.EnsureDeleted();
            }
        }
        
        [Theory]
        [InlineData(1)]
        public async Task GetByItemIdReturnsCorrect(int itemId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                ICommentRepository repository = new CommentRepository(context);
                //Act
                List<Comment> expected = await context.Comments.Where(r => r.ItemId == itemId).ToListAsync();
                IEnumerable<Comment> actual = await repository.GetByItemIdAsync(itemId);
                //Assert
                Assert.True(actual != null);
                Assert.Equal(expected.Count, actual.ToList().Count);
                context.Database.EnsureDeleted();
            }
        }
        
        [Theory]
        [InlineData(1)]
        public async Task Check_ReadCommentReturnSpecificComment(int commentId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                ICommentRepository repository = new CommentRepository(context);
                //Act
                var expected = await context.Comments.Where(r => r.Id == commentId).FirstOrDefaultAsync();
                Comment actual = await repository.ReadAsync(commentId);
                //Assert
                Assert.True(actual != null);
                Assert.Equal(expected, actual);
                context.Database.EnsureDeleted();
            }
        }
        
        [Theory]
        [InlineData(100)]
        public async Task Check_ReadNotExistCommentReturnNull(int commentId)
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                ICommentRepository repository = new CommentRepository(context);
                //Act
                var expected = await context.Comments.Where(r => r.Id == commentId).FirstOrDefaultAsync();
                Comment actual = await repository.ReadAsync(commentId);
                //Assert
                Assert.Null(actual);
                Assert.Equal(expected, actual);
                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public async Task Check_CreateCommentIsCorrect()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                ICommentRepository repository = new CommentRepository(context);
                //Act
                Comment commentNew = new Comment { Id = 10, ItemId = 1, Text = "Comment text10", UserId = "2138b181-4cee-4b85-9f16-18df308f387d", Date = DateTime.Today };
                await repository.CreateAsync(commentNew);
                var actual = context.Comments.Find(commentNew.Id);
                //Assert
                Assert.NotNull(actual);
                Assert.Equal(commentNew, actual);
                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public async Task Check_UpdateCommentIsCorrect()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                ICommentRepository repository = new CommentRepository(context);
                //Act
                Comment commentNew = context.Comments.FirstOrDefault();
                commentNew.Text = "NEW TEXT";
                await repository.UpdateAsync(commentNew);
                var actual = context.Comments.Find(commentNew.Id);
                //Assert
                Assert.NotNull(actual);
                Assert.Equal(commentNew, actual);
                Assert.Equal(commentNew.Text, actual.Text);
                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public async Task Check_UpdateNotCorrectCommentThrowsException()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                ICommentRepository repository = new CommentRepository(context);
                //Act
                Comment commentNew = context.Comments.FirstOrDefault();
                commentNew.Id = -2;
                //Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => repository.UpdateAsync(commentNew));
                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public async Task Check_DeleteCommentIsCorrect()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                ICommentRepository repository = new CommentRepository(context);
                //Act
                Comment existComment = context.Comments.FirstOrDefault();
                
                await repository.DeleteAsync(existComment.Id);
                
                var actual = context.Comments.Find(existComment.Id);
                //Assert
                Assert.NotNull(existComment);
                Assert.Null(actual);
                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public async Task Check_DeleteNotExistCommentThrowsException()
        {
            //Arrange
            var cls = new InMemoryAppDbContext();
            using (var context = cls.GetContextWithData())
            {
                ICommentRepository repository = new CommentRepository(context);
                //Act
                Comment existComment = context.Comments.FirstOrDefault();
                
                await Assert.ThrowsAsync<ArgumentNullException>(() => repository.DeleteAsync(existComment.Id-100));

                context.Database.EnsureDeleted();
            }
        }
    }
}