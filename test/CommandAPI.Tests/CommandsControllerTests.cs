using System;
using System.Collections.Generic;
using Moq;
using AutoMapper;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using CommandAPI.Controllers;
using CommandAPI.Models;
using CommandAPI.Data;
using CommandAPI.Profiles;
using CommandAPI.Dtos;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests : IDisposable
    {
        Mock<ICommandAPIRepo> mockRepo;
        CommandsProfile realProfile;
        MapperConfiguration configuration;
        IMapper mapper;

        public CommandsControllerTests()
        {
            mockRepo = new Mock<ICommandAPIRepo>();
            realProfile = new CommandsProfile();
            configuration = new MapperConfiguration(cfg => cfg.AddProfile(realProfile));
            mapper = new Mapper(configuration);
        }

        public void Dispose()
        {
            mockRepo = null;
            mapper = null;
            configuration = null;
            realProfile = null;
        }

        private List<Command> GetCommands(int num)
        {
            var commands = new List<Command>();
            if (num > 0)
            {
                commands.Add(new Command
                {
                    Id = 0,
                    HowTo = "How to generate migration",
                    CommandLine = "dotnet ef migrations add <Name of Migration>",
                    Platform = ".Net Core EF"
                });
            }

            return commands;
        }

        [Fact]
        public void GetCommandItems_Return200OK_WhenDBIsEmpty()
        {
            //Given
            mockRepo.Setup(repo => repo.GetAllCommands()).Returns(GetCommands(0));

            var controller = new CommandsController(mockRepo.Object, mapper);

            //When
            var result = controller.GetAllCommands();

            //Then
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetAllCommands_ReturnsOneItem_WhenDBHasOneResource()
        {
            //Given
            mockRepo.Setup(repo => repo.GetAllCommands()).Returns(GetCommands(1));
            var controller = new CommandsController(mockRepo.Object, mapper);

            //When
            var result = controller.GetAllCommands();

            //Then
            var okResult = result.Result as OkObjectResult;
            var commands = okResult.Value as List<CommandReadDto>;
            Assert.Single(commands);
        }

        [Fact]
        public void GetAllCommands_Returns200OK_WhenDBHasOneResource()
        {
            //Given
            mockRepo.Setup(repo => repo.GetAllCommands()).Returns(GetCommands(1));
            var controller = new CommandsController(mockRepo.Object, mapper);

            //When
            var result = controller.GetAllCommands();

            //Then
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetAllCommands_ReturnsCorrectType_WhenDBHasOneResource()
        {
            //Given
            mockRepo.Setup(repo => repo.GetAllCommands()).Returns(GetCommands(1));
            var controller = new CommandsController(mockRepo.Object, mapper);

            //When
            var result = controller.GetAllCommands();

            //Then
            Assert.IsType<ActionResult<IEnumerable<CommandReadDto>>>(result);
        }

        [Fact]
        public void GetCommandById_ReturnsNotFound_WhenNonExistentIdProvided()
        {
            //Given
            mockRepo.Setup(repo => repo.GetCommandById(0)).Returns(() => null);
            var controller = new CommandsController(mockRepo.Object, mapper);

            //When
            var result = controller.GetCommandById(1);

            //Then
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetCommandById_ReturnsOk_WhenValidIdProvided()
        {
            //Given
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(() => new Command
            {
                Id = 1,
                HowTo = "mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });
            var controller = new CommandsController(mockRepo.Object, mapper);

            //When
            var result = controller.GetCommandById(1);

            //Then
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetCommandById_ReturnsCorrectType_WhenValidIdProvided()
        {
            //Given
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(() => new Command
            {
                Id = 1,
                HowTo = "mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });
            var controller = new CommandsController(mockRepo.Object, mapper);

            //When
            var result = controller.GetCommandById(1);

            //Then
            Assert.IsType<ActionResult<CommandReadDto>>(result);
        }

        [Fact]
        public void CreateCommand_ReturnsCorrectResourceType_WhenValidObjectSubmitted()
        {
            //Arrange
            mockRepo.Setup(repo =>
            repo.GetCommandById(1)).Returns(new Command
            {
                Id = 1,
                HowTo = "mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });
            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act
            var result = controller.CreateCommand(new CommandCreateDto { });
            //Assert
            Assert.IsType<ActionResult<CommandReadDto>>(result);
        }

        [Fact]
        public void CreateCommand_Returns201Created_WhenValidObjectSubmitted()
        {
            //Arrange
            mockRepo.Setup(repo =>
            repo.GetCommandById(1)).Returns(new Command
            {
                Id = 1,
                HowTo = "mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });
            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act
            var result = controller.CreateCommand(new CommandCreateDto { });
            //Assert
            Assert.IsType<CreatedAtRouteResult>(result.Result);
        }

        [Fact]
        public void UpdateCommand_Returns204NoContent_WhenValidObjectSubmitted()
        {
            //Arrange
            mockRepo.Setup(repo =>
            repo.GetCommandById(1)).Returns(new Command
            {
                Id = 1,
                HowTo = "mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });
            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act
            var result = controller.UpdateCommand(1, new CommandUpdateDto { });
            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateCommand_Returns404NotFound_WhenNonExistentResourceIDSubmitted()
        {
            //Arrange
            mockRepo.Setup(repo =>
            repo.GetCommandById(0)).Returns(() => null);
            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act
            var result = controller.UpdateCommand(0, new CommandUpdateDto { });
            //Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}