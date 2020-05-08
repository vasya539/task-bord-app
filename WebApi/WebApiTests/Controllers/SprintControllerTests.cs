using System;
using Xunit;
using WebApi.Controllers;
using WebApi.BLs.Interfaces;
using Moq;
using WebApi.Data.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.BLs.Communication;

namespace WebApiTests
{
	public class SprintControllerTests
	{
		private readonly SprintController _sprintController;
		private readonly Mock<ISprintBl> _mockSprintBL;
		public SprintControllerTests()
		{
			_mockSprintBL = new Mock<ISprintBl>();
			_sprintController = new SprintController(_mockSprintBL.Object);
		}

		[Fact]
		public void GetAllByProjectIdAsync_ReturnsRightAmountOfUsers()
		{
			//Arrange
			_mockSprintBL.Setup(bl => bl.GetAllByProjectIdAsync(5, new String("NotFound")))
				.Returns(Task.FromResult((IEnumerable<SprintDto>)new List<SprintDto>()
				{
					new SprintDto {Id =1, Items=null, ProjectId=1, StartDate=new DateTime(2020,2,26), EndDate=new DateTime(2020,2,26) },
					new SprintDto {Id =2, Items=null, ProjectId=1, StartDate=new DateTime(2020,2,26), EndDate=new DateTime(2020,2,26) },
					new SprintDto {Id =3, Items=null, ProjectId=1, StartDate=new DateTime(2020,2,26), EndDate=new DateTime(2020,2,26) },
					new SprintDto {Id =4, Items=null, ProjectId=1, StartDate=new DateTime(2020,2,26), EndDate=new DateTime(2020,2,26) },
					new SprintDto {Id =5, Items=null, ProjectId=1, StartDate=new DateTime(2020,2,26), EndDate=new DateTime(2020,2,26) }
				}));

			//Act
			var result = _sprintController.GetAllByProjectIdAsync(5).Result;

			//Asserrt
			Assert.Equal(5, (result as List<SprintDto>).Count);
		}

		[Fact]
		public void GetAllByProjectIdAsync_ReturnsAppropriateType()
		{
			//Arrange
			_mockSprintBL.Setup(bl => bl.GetAllByProjectIdAsync(5, new String("NotFound")))
				.Returns(Task.FromResult((IEnumerable<SprintDto>)new List<SprintDto>()
				{
					new SprintDto {Id =1, Items=null, ProjectId=1, StartDate=new DateTime(2020,2,26), EndDate=new DateTime(2020,2,26) },
					new SprintDto {Id =2, Items=null, ProjectId=1, StartDate=new DateTime(2020,2,26), EndDate=new DateTime(2020,2,26) },
					new SprintDto {Id =3, Items=null, ProjectId=1, StartDate=new DateTime(2020,2,26), EndDate=new DateTime(2020,2,26) },
					new SprintDto {Id =4, Items=null, ProjectId=1, StartDate=new DateTime(2020,2,26), EndDate=new DateTime(2020,2,26) },
					new SprintDto {Id =5, Items=null, ProjectId=1, StartDate=new DateTime(2020,2,26), EndDate=new DateTime(2020,2,26) }
				}));

			//Act
			var result = _sprintController.GetAllByProjectIdAsync(5).Result;

			//Asserrt
			Assert.IsAssignableFrom<IEnumerable<SprintDto>>(result);
		}

		[Fact]
		public void GetAsync_ReturnsSprintWithCorrectId()
		{
			//Arrange
			_mockSprintBL.Setup(bl => bl.GetByIdAsync(5, new String("NotFound")))
				.Returns(Task.FromResult(new SprintDto
					{Id =5, Items=null, ProjectId=1, StartDate=new DateTime(2020,2,26), EndDate=new DateTime(2020,2,26) }
				));

			//Act
			var okResult = ((_sprintController.GetAsync(5).Result as OkObjectResult).Value) as SprintDto;
			
			//Asserrt
			Assert.Equal(5, okResult.Id);
		}

		[Fact]
		public void GetAsync_ReturnsCorrectType()
		{
			//Arrange
			_mockSprintBL.Setup(bl => bl.GetByIdAsync(5, new String("NotFound")))
				.Returns(Task.FromResult(new SprintDto
				{ Id = 5, Items = null, ProjectId = 1, StartDate = new DateTime(2020, 2, 26), EndDate = new DateTime(2020, 2, 26) }
				));

			//Act
			var result = _sprintController.GetAsync(5).Result;

			//Asserrt
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public void GetAsync_ReturnsBadRequestWhenNull()
		{
			//Arrange
			_mockSprintBL.Setup(bl => bl.GetByIdAsync(5, new String("NotFound")))
				.Returns(Task.FromResult((SprintDto)null));

			//Act
			var result = _sprintController.GetAsync(5).Result;

			//Asserrt
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public void PostAsync_ReturnsOkResult()
		{
			//Arrange
			SprintDto dto = new SprintDto {Id=0, Items=null, Name="Sprint 1", Description="Wow", ProjectId = 1, StartDate = new DateTime(2020, 2, 26), EndDate = new DateTime(2020, 2, 26) };
			_mockSprintBL.Setup(bl => bl.CreateAsync(dto, new String("NotFound")))
				.Returns(Task.FromResult(
				new SprintResponse(
					new SprintDto
					{
						Id = 20,
						Name = "Sprint 1",
						Description = "Wow",
						ProjectId = 1,
						Items = null,
						StartDate = new DateTime(2020, 2, 26),
						EndDate = new DateTime(2020, 2, 26)
					})
				));

			//Act
			var result = _sprintController.PostAsync(dto).Result;

			//Asserrt
			Assert.IsType<OkResult>(result);

		}

		[Fact]
		public void PostAsync_ReturnsBadRequestResultOnError()
		{
			//Arrange
			SprintDto dto = new SprintDto { Id = 0, Items = null, Name = "Sprint 1", Description = "Wow", ProjectId = 1, StartDate = new DateTime(2020, 2, 26), EndDate = new DateTime(2020, 2, 26) };
			_mockSprintBL.Setup(bl => bl.CreateAsync(dto, new String("NotFound")))
				.Returns(Task.FromResult(
				new SprintResponse(message: "Cannot add new sprint")));

			//Act
			var result = _sprintController.PostAsync(dto).Result;

			//Asserrt
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public void PostAsync_ReturnsCorrectErrorMessage()
		{
			//Arrange
			SprintDto dto = new SprintDto { ProjectId = 1, StartDate = new DateTime(2020, 2, 26), EndDate = new DateTime(2020, 2, 26) };
			_mockSprintBL.Setup(bl => bl.CreateAsync(dto, new String("NotFound")))
				.Returns(Task.FromResult(
				new SprintResponse(message: "Cannot add new sprint")));

			//Act
			var result = _sprintController.PostAsync(dto).Result;

			//Asserrt
			Assert.Equal("Cannot add new sprint", (result as BadRequestObjectResult).Value);
		}

		[Fact]
		public void PutAsync_ReturnsOkResult()
		{
			//Arrange
			SprintDto dto = new SprintDto {Id=1, ProjectId = 1, Items=null, StartDate = new DateTime(2020, 2, 26), EndDate = new DateTime(2020, 2, 26) };
			_mockSprintBL.Setup(bl => bl.UpdateAsync(dto, new String("NotFound")))
				.Returns(Task.FromResult(
				new SprintResponse(
					new SprintDto
					{
						Id = 20,
						ProjectId = 1,
						Items = null,
						StartDate = new DateTime(2020, 2, 26),
						EndDate = new DateTime(2020, 2, 26)
					})
				));

			//Act
			var result = _sprintController.PutAsync(dto).Result;

			//Asserrt
			Assert.IsType<OkResult>(result);
		}

		[Fact]
		public void PutAsync_ReturnsBadRequestResult()
		{
			//Arrange
			SprintDto dto = new SprintDto { Id = 1, ProjectId = 1, Items = null, StartDate = new DateTime(2020, 2, 26), EndDate = new DateTime(2020, 2, 26) };
			_mockSprintBL.Setup(bl => bl.UpdateAsync(dto, new String("NotFound")))
				.Returns(Task.FromResult(
				new SprintResponse(message: "An error occurred on updating the sprint")));

			//Act
			var result = _sprintController.PutAsync(dto).Result;

			//Asserrt
			Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("An error occurred on updating the sprint", (result as BadRequestObjectResult).Value);
		}

		[Fact]
		public void DeleteAsync_ReturnsCorrectType()
		{
			//Arrange
			_mockSprintBL.Setup(bl => bl.DeleteAsync(5, new String("NotFound")))
				.Returns(Task.FromResult(
				new SprintResponse(
					new SprintDto
					{
						Id = 5,
						ProjectId = 1,
						Items = null,
						StartDate = new DateTime(2020, 2, 26),
						EndDate = new DateTime(2020, 2, 26)
					})
				));

			//Act
			var result = _sprintController.DeleteAsync(5).Result;

			//Asserrt
			Assert.IsType<OkResult>(result);
		}

		[Fact]
		public void DeleteAsync_ReturnsBadRequestResult()
		{
			//Arrange
			_mockSprintBL.Setup(bl => bl.DeleteAsync(5, new String("NotFound")))
				.Returns(Task.FromResult(
				new SprintResponse(message: "An error occurred on deleting the sprint")));

			//Act
			var result = _sprintController.DeleteAsync(5).Result;

			//Asserrt
			Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("An error occurred on deleting the sprint", (result as BadRequestObjectResult).Value);
		}

		[Fact]
		public void GetAllSprintItemsAsync_ReturnsOkResult()
		{
			//Arrange
			_mockSprintBL.Setup(bl => bl.GetAllSprintItemsAsync(1, new String("NotFound")))
				.Returns(Task.FromResult(new ItemListDto[] {
					new ItemListDto { Id = 1, ParentId = null, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1 },
					new ItemListDto { Id = 2, ParentId = null, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name2", Description = "Description Item2", StatusId = 2, TypeId = 2 },
					new ItemListDto { Id = 3, ParentId = null, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name3", Description = "Description Item3", StatusId = 3, TypeId = 1 },
					new ItemListDto { Id = 4, ParentId = null, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name4", Description = "Description Item4", StatusId = 1, TypeId = 2 }
				} as IEnumerable<ItemListDto>));

			//Act
			var result = _sprintController.GetAllSprintItemsAsync(1).Result;

			//Asserrt
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public void GetAllSprintItemsAsync_ReturnsNotFoundResult()
		{
			//Arrange
			_mockSprintBL.Setup(bl => bl.GetAllSprintItemsAsync(1, new String("NotFound")))
				.Returns(Task.FromResult(null as IEnumerable<ItemListDto>));

			//Act
			var result = _sprintController.GetAllSprintItemsAsync(1).Result;

			//Asserrt
			Assert.IsType<NotFoundResult>(result);
		}

	}
}
