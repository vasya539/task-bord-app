using BusinessLogicLayer.Tests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.BLs.Communication;
using WebApi.BLs.Interfaces;
using WebApi.Controllers;
using WebApi.Data.DTOs;
using Xunit;

namespace WebApiTests.Controllers
{
    //Arrange
    //Act
    //Assert\

    public class ItemControllerTest
    {
        private readonly Mock<IItemBl> _mockItemBl;
        private readonly Mock<ICommentBl> _mockCommentBl;
        private readonly Mock<IItemRelationBl> _mockItemRelationBl;

        public ItemControllerTest()
        {
            _mockItemBl = new Mock<IItemBl>();
            _mockCommentBl = new Mock<ICommentBl>();
            _mockItemRelationBl = new Mock<IItemRelationBl>();
        }

        #region Get-methods

        #region Get all items

        [Fact]
        public async Task GetAllItemsThatReturnsItemDtoList()
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetAllAsync()).ReturnsAsync(GetTestItems());
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetAllAsync() as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(viewResult.Value);
            _mockItemBl.Verify(r => r.GetAllAsync(), Times.Once);
            Assert.Equal(GetTestItems().Count(), model.Count());
        }

        [Fact]
        public async Task GetAllItemsThatReturnsNotFound()
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetAllAsync()).ReturnsAsync(null as IEnumerable<ItemDto>);
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetAllAsync() as NotFoundResult;
            // Assert
            _mockItemBl.Verify(r => r.GetAllAsync(), Times.Once);
            var viewResult = Assert.IsType<NotFoundResult>(res1);
        }

        #endregion Get all items

        #region Get all by sprintId

        [Theory]
        [InlineData(2)]
        public async Task GetAllBySprintIdReturnsItemDtos(int sprintId)
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetBySprintIdAsync(sprintId)).ReturnsAsync(GetTestItems());
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetAllBySprintIdAsync(sprintId) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(viewResult.Value);
            _mockItemBl.Verify(r => r.GetBySprintIdAsync(sprintId), Times.Once);
            Assert.Equal(GetTestItems().Count(), model.Count());
        }

        [Theory]
        [InlineData(2)]
        public async Task GetAllBySprintIdReturnsNotFound(int sprintId)
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetBySprintIdAsync(sprintId)).ReturnsAsync(null as IEnumerable<ItemDto>);
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetAllBySprintIdAsync(sprintId) as NotFoundResult;
            // Assert
            _mockItemBl.Verify(r => r.GetBySprintIdAsync(sprintId), Times.Once);
            var viewResult = Assert.IsType<NotFoundResult>(res1);
        }

        #endregion Get all by sprintId

        #region Get archivated by sprintId

        [Theory]
        [InlineData(2)]
        public async Task GetArchivatedBySprintIdReturnsItemDtos(int sprintId)
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetArchivedBySprintIdAsync(sprintId)).ReturnsAsync(GetTestItems());
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetArchivedBySprintIdAsync(sprintId) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(viewResult.Value);
            _mockItemBl.Verify(r => r.GetArchivedBySprintIdAsync(sprintId), Times.Once);
            Assert.Equal(GetTestItems().Count(), model.Count());
        }

        [Theory]
        [InlineData(2)]
        public async Task GetArchivateBySprintIdReturnsNotFound(int sprintId)
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetArchivedBySprintIdAsync(sprintId)).ReturnsAsync(null as IEnumerable<ItemDto>);
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetArchivedBySprintIdAsync(sprintId) as NotFoundResult;
            // Assert
            _mockItemBl.Verify(r => r.GetArchivedBySprintIdAsync(sprintId), Times.Once);
            var viewResult = Assert.IsType<NotFoundResult>(res1);
        }

        #endregion Get archivated by sprintId

        #region Get childs for item

        [Theory]
        [InlineData(2)]
        public async Task GetAllChildsByItemIdReturnsItemDtos(int itemId)
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetAllChildAsync(itemId)).ReturnsAsync(GetTestItems());
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetAllChildAsync(itemId) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(viewResult.Value);
            _mockItemBl.Verify(r => r.GetAllChildAsync(itemId), Times.Once);
            Assert.Equal(GetTestItems().Count(), model.Count());
        }

        [Theory]
        [InlineData(2)]
        public async Task GetAllChildsByItemIdReturnsNotFound(int itemId)
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetAllChildAsync(itemId)).ReturnsAsync(null as IEnumerable<ItemDto>);
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetAllChildAsync(itemId) as NotFoundResult;
            // Assert
            _mockItemBl.Verify(r => r.GetAllChildAsync(itemId), Times.Once);
            var viewResult = Assert.IsType<NotFoundResult>(res1);
        }

        #endregion Get childs for item

        #region Get unparented  items by sprintId

        [Theory]
        [InlineData(2)]
        public async Task GetAllUnparentedBySprintIdReturnsItemDtos(int sprintId)
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetUnparentedAsync(sprintId)).ReturnsAsync(GetTestItems());
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetAllUnparentedAsync(sprintId) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(viewResult.Value);
            _mockItemBl.Verify(r => r.GetUnparentedAsync(sprintId), Times.Once);
            Assert.Equal(GetTestItems().Count(), model.Count());
        }

        [Theory]
        [InlineData(2)]
        public async Task GetAllUnparentedBySprintIdReturnsNotFound(int sprintId)
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetUnparentedAsync(sprintId)).ReturnsAsync(null as IEnumerable<ItemDto>);
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetAllUnparentedAsync(sprintId) as NotFoundResult;
            // Assert
            _mockItemBl.Verify(r => r.GetUnparentedAsync(sprintId), Times.Once);
            var viewResult = Assert.IsType<NotFoundResult>(res1);
        }

        #endregion Get unparented  items by sprintId


        [Theory]
        [InlineData(2)]
        public async Task GetAllUserstoriesBySprintIdReturnsItemDtos(int sprintId)
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetUserStoriesAsync(sprintId)).ReturnsAsync(GetTestItems());
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetAllUserStoriesAsync(sprintId) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(viewResult.Value);
            _mockItemBl.Verify(r => r.GetUserStoriesAsync(sprintId), Times.Once);
            Assert.Equal(GetTestItems().Count(), model.Count());
        }

        [Theory]
        [InlineData(2)]
        public async Task GetAllUserstoriesBySprintIdReturnsNotFound(int sprintId)
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetUserStoriesAsync(sprintId)).ReturnsAsync(null as IEnumerable<ItemDto>);
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetAllUserStoriesAsync(sprintId) as NotFoundResult;
            // Assert
            _mockItemBl.Verify(r => r.GetUserStoriesAsync(sprintId), Times.Once);
            var viewResult = Assert.IsType<NotFoundResult>(res1);
        }

        [Theory]
        [InlineData(2)]
        public async Task GetAllCommentsByItemIdReturnsComments(int itemId)
        {
            // Arrange
            _mockCommentBl.Setup(repo => repo.GetByItemIdAsync(itemId)).ReturnsAsync(GetTestComments());
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetAllCommentsByItemIdAsync(itemId) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<IEnumerable<CommentDto>>(viewResult.Value);
            _mockCommentBl.Verify(r => r.GetByItemIdAsync(itemId), Times.Once);
            Assert.Equal(GetTestComments().Count(), model.Count());
        }
        
        [Theory]
        [InlineData(2)]
        public async Task GetAllCommentsByItemIdReturnsNotFound(int itemId)
        {
            // Arrange
            _mockCommentBl.Setup(repo => repo.GetByItemIdAsync(itemId)).ReturnsAsync(null as IEnumerable<CommentDto>);
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetAllCommentsByItemIdAsync(itemId) as NotFoundResult;
            // Assert
             Assert.IsType<NotFoundResult>(res1);
            _mockCommentBl.Verify(r => r.GetByItemIdAsync(itemId), Times.Once);
        }
        
        [Theory]
        [InlineData(2,5)]
        public async Task GetChildWithSpecificStatusReturnsItemDtos(int itemId, int statusId)
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetChildWithSpecificStatusAsync(itemId, statusId)).ReturnsAsync(GetTestItems());
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetChildWithSpecificStatusAsync(itemId, statusId) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(viewResult.Value);
            _mockItemBl.Verify(r => r.GetChildWithSpecificStatusAsync(itemId, statusId), Times.Once);
            Assert.Equal(GetTestItems().Count(), model.Count());
        }
        
        [Theory]
        [InlineData(2,5)]
        public async Task GetChildWithSpecificStatusReturnsNotFound(int itemId, int statusId)
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.GetChildWithSpecificStatusAsync(itemId, statusId)).ReturnsAsync(null as IEnumerable<ItemDto>);
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetChildWithSpecificStatusAsync(itemId, statusId) as NotFoundResult;
            // Assert
            _mockItemBl.Verify(r => r.GetChildWithSpecificStatusAsync(itemId, statusId), Times.Once);
            Assert.IsType<NotFoundResult>(res1);
        }
        
        [Theory]
        [InlineData(2)]
        public async Task GetRelatedItemsReturnsItemDtos(int itemId)
        {
            // Arrange
            _mockItemRelationBl.Setup(repo => repo.GetRelatedItemsAsync(itemId)).ReturnsAsync(GetTestItems());
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetRelatedItemsAsync(itemId) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(viewResult.Value);
            _mockItemRelationBl.Verify(r => r.GetRelatedItemsAsync(itemId), Times.Once);
            Assert.Equal(GetTestItems().Count(), model.Count());
        }
        
        [Theory]
        [InlineData(2)]
        public async Task GetRelatedItemsReturnsNotFound(int itemId)
        {
            // Arrange
            _mockItemRelationBl.Setup(repo => repo.GetRelatedItemsAsync(itemId)).ReturnsAsync(null as IEnumerable<ItemDto>);
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetRelatedItemsAsync(itemId) as NotFoundResult;
            // Assert
            _mockItemRelationBl.Verify(r => r.GetRelatedItemsAsync(itemId), Times.Once);
            Assert.IsType<NotFoundResult>(res1);
        }
        
        #endregion Get-methods

        #region Read

        [Fact]
        public async Task GetOneItemReturnsItemDto()
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems().First);
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetItemAsync(It.IsAny<int>()) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<ItemDto>(viewResult.Value);
            _mockItemBl.Verify(r => r.ReadAsync(It.IsAny<int>()), Times.Once);
            Assert.Equal(GetTestItems().First(), model);
        }

        [Fact]
        public async Task GetOneItemReturnNotFound()
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.ReadAsync(It.IsAny<int>())).ReturnsAsync(null as ItemDto);
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.GetItemAsync(It.IsAny<int>()) as NotFoundResult;
            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(res1);
            _mockItemBl.Verify(r => r.ReadAsync(It.IsAny<int>()), Times.Once);
        }

        #endregion Read

        #region Create

        [Fact]
        public async Task CreateCorrectItemReturnsOk()
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.CreateAsync(GetTestItems().First(), It.IsAny<string>())).ReturnsAsync(new ItemResponse(true, "Created"));
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.CreateItemAsync(GetTestItems().First()) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<String>(viewResult.Value);
            _mockItemBl.Verify(r => r.CreateAsync(GetTestItems().First(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateUnloginedUserForbiddenReturnsBadRequest()
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.CreateAsync(GetTestItems().First(), It.IsAny<string>())).ReturnsAsync(new ItemResponse(false, "Forbidden"));
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.CreateItemAsync(GetTestItems().First()) as BadRequestObjectResult;
            // Assert

            var viewResult = Assert.IsType<BadRequestObjectResult>(res1);
            var model = Assert.IsAssignableFrom<String>(viewResult.Value);
            Assert.Equal("Forbidden", viewResult.Value);
            _mockItemBl.Verify(r => r.CreateAsync(GetTestItems().First(), It.IsAny<string>()), Times.Once);
        }

        #endregion Create

        #region Update

        [Fact]
        public async Task UpdateCorrectItemReturnsOk()
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.UpdateAsync(GetTestItems().First(), It.IsAny<string>())).ReturnsAsync(new ItemResponse(true, "Created"));
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.UpdateItemAsync(GetTestItems().First()) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<String>(viewResult.Value);
            Assert.Equal("Created", viewResult.Value);
            _mockItemBl.Verify(r => r.UpdateAsync(GetTestItems().First(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUnloginedUserForbiddenReturnsBadRequest()
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.UpdateAsync(GetTestItems().First(), It.IsAny<string>())).ReturnsAsync(new ItemResponse(false, "Forbidden"));
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.UpdateItemAsync(GetTestItems().First()) as BadRequestObjectResult;
            // Assert

            var viewResult = Assert.IsType<BadRequestObjectResult>(res1);
            var model = Assert.IsAssignableFrom<String>(viewResult.Value);
            Assert.Equal("Forbidden", viewResult.Value);
            _mockItemBl.Verify(r => r.UpdateAsync(GetTestItems().First(), It.IsAny<string>()), Times.Once);
        }

        //[Fact]
        //public async Task CreateItemCorrectReturnsOk()
        //{
        //    Mock<ControllerBase> _basec = new Mock<ControllerBase>();
        //    _basec.Setup(r => r.User.Claims.SingleOrDefault(c => c.Type == "uid").Value).Returns("userId");
        //    // Arrange
        //    _mockItemBl.Setup(repo => repo.CreateAsync(GetTestItems().First(), It.IsAny<string>())).ReturnsAsync(It.IsAny<ItemResponse>());
        //    var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object);
        //    // Act
        //    var res1 = await itemController.CreateItem(GetTestItems().First()) as OkObjectResult;
        //    // Assert
        //    var viewResult = Assert.IsType<OkObjectResult>(res1);
        //    var model = Assert.IsAssignableFrom<ItemDto>(viewResult.Value);
        //    _mockItemBl.Verify(r => r.ReadAsync(It.IsAny<int>()), Times.Once);
        //    Assert.Equal(GetTestItems().First(), model);
        //}

        #endregion Update

        #region Delete(Archive)

        [Fact]
        public async Task ArchivateCorrectItemReturnsOk()
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.ArchivingAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new ItemResponse(true, "Created"));
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.ArchiveItemAsync(It.IsAny<int>()) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<string>(viewResult.Value);
            Assert.Equal("Created", viewResult.Value);
            _mockItemBl.Verify(r => r.ArchivingAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ArchivateUnloginedUserReturnsBadRequest()
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.ArchivingAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new ItemResponse(false, "Forbidden"));
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.ArchiveItemAsync(It.IsAny<int>()) as BadRequestObjectResult;
            // Assert

            var viewResult = Assert.IsType<BadRequestObjectResult>(res1);
            var model = Assert.IsAssignableFrom<string>(viewResult.Value);
            Assert.Equal("Forbidden", viewResult.Value);
            _mockItemBl.Verify(r => r.ArchivingAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCorrectItemReturnsOk()
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.DeleteAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new ItemResponse(true, "Created"));
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.DeleteItemAsync(It.IsAny<int>()) as OkObjectResult;
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(res1);
            var model = Assert.IsAssignableFrom<string>(viewResult.Value);
            Assert.Equal("Created", viewResult.Value);
            _mockItemBl.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUnloginedUserReturnsBadRequest()
        {
            // Arrange
            _mockItemBl.Setup(repo => repo.DeleteAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new ItemResponse(false, "Forbidden"));
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.DeleteItemAsync(It.IsAny<int>()) as BadRequestObjectResult;
            // Assert

            var viewResult = Assert.IsType<BadRequestObjectResult>(res1);
            var model = Assert.IsAssignableFrom<string>(viewResult.Value);
            Assert.Equal("Forbidden", viewResult.Value);
            _mockItemBl.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        #endregion Delete(Archive)
        
        [Theory]
        [InlineData(2,10)]
        public async Task RelateItemsCorrectReturnsOk(int firstItemId, int secondItemId)
        {
            // Arrange
            _mockItemRelationBl.Setup(repo => repo.CreateRecordAsync(firstItemId,secondItemId, It.IsAny<string>())).ReturnsAsync(new ItemResponse(true, "Created"));
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.RelateItemsAsync(firstItemId,secondItemId) as OkObjectResult;
            // Assert
            Assert.IsType<OkObjectResult>(res1);
            _mockItemRelationBl.Verify(r => r.CreateRecordAsync(firstItemId,secondItemId, It.IsAny<string>()), Times.Once);
        }
        
        [Theory]
        [InlineData(2,10)]
        public async Task RelateItemsNotCorrectReturnsBadRequest(int firstItemId, int secondItemId)
        {
            // Arrange
            _mockItemRelationBl.Setup(repo => repo.CreateRecordAsync(firstItemId,secondItemId, It.IsAny<string>())).ReturnsAsync(new ItemResponse(false, "Created"));
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.RelateItemsAsync(firstItemId,secondItemId) as BadRequestObjectResult;
            // Assert
            Assert.IsType<BadRequestObjectResult>(res1);
            _mockItemRelationBl.Verify(r => r.CreateRecordAsync(firstItemId,secondItemId, It.IsAny<string>()), Times.Once);
        }
        
        [Theory]
        [InlineData(2,10)]
        public async Task DeleteRelationCorrectReturnsOk(int firstItemId, int secondItemId)
        {
            // Arrange
            _mockItemRelationBl.Setup(repo => repo.DeleteRecordAsync(firstItemId,secondItemId, It.IsAny<string>())).ReturnsAsync(new ItemResponse(true, "Created"));
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.DeleteRelationBetweenItemsAsync(firstItemId,secondItemId) as OkObjectResult;
            // Assert
            Assert.IsType<OkObjectResult>(res1);
            _mockItemRelationBl.Verify(r => r.DeleteRecordAsync(firstItemId,secondItemId, It.IsAny<string>()), Times.Once);
        }
        
        [Theory]
        [InlineData(2,10)]
        public async Task DeleteRelationCorrectReturnBadRequest(int firstItemId, int secondItemId)
        {
            // Arrange
            _mockItemRelationBl.Setup(repo => repo.DeleteRecordAsync(firstItemId,secondItemId, It.IsAny<string>())).ReturnsAsync(new ItemResponse(false, "Created"));
            var itemController = new ItemController(_mockItemBl.Object, _mockCommentBl.Object, _mockItemRelationBl.Object);
            // Act
            var res1 = await itemController.DeleteRelationBetweenItemsAsync(firstItemId,secondItemId) as BadRequestObjectResult;
            // Assert
            Assert.IsType<BadRequestObjectResult>(res1);
            _mockItemRelationBl.Verify(r => r.DeleteRecordAsync(firstItemId,secondItemId, It.IsAny<string>()), Times.Once);
        }
        
        public IEnumerable<ItemDto> GetTestItems()
        {
            var items = new List<ItemDto>
            {
                new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev2", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = null, StoryPoint = 2 },
                new ItemDto { Id = 2, SprintId = 1, AssignedUserId = "dev3", Name = "Item Name2", Description = "Description Item2", StatusId = 2, TypeId = 2, IsArchived = false, ParentId = 1, StoryPoint = 2 },
                new ItemDto { Id = 3, SprintId = 2, AssignedUserId = "421cb65f-a76d-4a73-8a1a-d792f37ef992", Name = "Item Name3", Description = "Description Item3", StatusId = 3, TypeId = 1, IsArchived = true, ParentId = 1, StoryPoint = 2 },
                new ItemDto { Id = 4, SprintId = 2, AssignedUserId = "54bfd1f9-d379-4930-9c3b-4c84992c028e", Name = "Item Name4", Description = "Description Item4", StatusId = 3, TypeId = 1, IsArchived = false, ParentId = 2, StoryPoint = 2 },
                new ItemDto { Id = 5, SprintId = 2, AssignedUserId = "54bfd1f9-d379-4930-9c3b-4c84992c028e", Name = "Item Name5", Description = "Description Item5", StatusId = 3, TypeId = 1, IsArchived = true, ParentId = null, StoryPoint = 2 },
                new ItemDto { Id = 6, SprintId = 2, AssignedUserId = null, Name = "Item Name6", Description = "Description Item5", StatusId = 2, TypeId = 2, IsArchived = false, ParentId = null, StoryPoint = 2 },
                new ItemDto { Id = 7, SprintId = 2, AssignedUserId = "dev", Name = "Item Name7", Description = "Description Item5", StatusId = 3, TypeId = 2, IsArchived = false, ParentId = null, StoryPoint = 2 },
                new ItemDto { Id =8, SprintId = 2, AssignedUserId = null, Name = "Item Name7", Description = "Description Item5", StatusId = 1, TypeId = 2, IsArchived = false, ParentId = null, StoryPoint = 2 }
            };
            return items;
        }

        public IEnumerable<CommentDto> GetTestComments()
        {
            var comments = new List<CommentDto>
            {
                new CommentDto{Id = 1, ItemId = 1, Text = "Comment text 1", UserId = "2138b181-4cee-4b85-9f16-18df308f387d", Date = DateTime.Today},
                new CommentDto{Id = 2, ItemId = 1, Text = "Comment text 2", UserId = "2138b181-4cee-4b85-9f16-18df308f387d", Date = DateTime.Today},
            };
            return comments;
        } 
    }
}