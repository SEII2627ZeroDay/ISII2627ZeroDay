using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.GameDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT.GamesController_test
{
    public class CreateGame_test : AppForSEII4SqliteUT
    {
        public CreateGame_test()
        {
            //SeedData
            var football = _context.Sports.FirstOrDefault(s => s.Name == "Football");
            if (football == null)
            {
                football = new Sport("Football", 11, 4, "Football match", "Standard rules");
                _context.Sports.Add(football);
                _context.SaveChanges();
            }

            var peter = _context.Users.FirstOrDefault(u => u.UserName == "peter@uclm.es") as ApplicationUser;
            if (peter == null)
            {
                //Constructor ApplicationUser
                peter = new ApplicationUser("peter-id", "Peter", "Jackson", "peter@uclm.es", new DateOnly(1998, 5, 20), 28, Gender.Male);
                _context.Users.Add(peter);
                _context.SaveChanges();
            }

            var elena = _context.Users.FirstOrDefault(u => u.UserName == "elena@uclm.es") as ApplicationUser;
            if (elena == null)
            {
                elena = new ApplicationUser("elena-id", "Elena", "Navarro", "elena@uclm.es", new DateOnly(1985, 3, 15), 41, Gender.Female);
                _context.Users.Add(elena);
                _context.SaveChanges();
            }

            var eaglesTeam = new Team("Eagles", "University football team", 15, 18, 35, peter!, football);
            _context.Teams.Add(eaglesTeam);

            var existingGame = new Game("Duplicate Game Name", new DateTime(2026, 11, 25), "Stadium", "Desc", football, elena!);
            _context.Games.Add(existingGame);

            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateGame_Success()
        {
            //Arrange
            var mock = new Mock<ILogger<GamesController>>();
            var controller = new GamesController(_context, mock.Object);

            var invitationsDto = new List<GameInvitationDTO> { new GameInvitationDTO("Eagles", false, "We invite you!") };
            var gameForCreate = new GameForCreateDTO(
                "New Unique Game",
                new DateTime(2026, 11, 30, 20, 0, 0),
                "Main Pavilion",
                "Friendly Match",
                "Football",
                "elena@uclm.es",
                invitationsDto
            );

            //Act
            var result = await controller.CreateGame(gameForCreate);

            //Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualDto = Assert.IsType<GameDetailDTO>(createdResult.Value);
            
            Assert.Equal("New Unique Game", actualDto.Name);
            Assert.Equal("Football", actualDto.SportName);
            Assert.Equal("elena@uclm.es", actualDto.ResponsibleUserName);
            Assert.Single(actualDto.GameInvitations);
            Assert.Equal("Eagles", actualDto.GameInvitations[0].TeamName);
        }

        [Fact]
        public async Task CreateGame_DuplicateName()
        {
            //Arrange
            var mock = new Mock<ILogger<GamesController>>();
            var controller = new GamesController(_context, mock.Object);

            var gameForCreate = new GameForCreateDTO(
                "Duplicate Game Name", 
                new DateTime(2026, 11, 30),
                "Main Pavilion",
                "Friendly Match",
                "Football",
                "elena@uclm.es",
                new List<GameInvitationDTO>()
            );

            //Act
            var result = await controller.CreateGame(gameForCreate);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("A game with this name already exists.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateGame_InvalidSport()
        {
            //Arrange
            var mock = new Mock<ILogger<GamesController>>();
            var controller = new GamesController(_context, mock.Object);

            var gameForCreate = new GameForCreateDTO(
                "Game With Bad Sport",
                new DateTime(2026, 11, 30),
                "Main Pavilion",
                "Friendly Match",
                "Non-Existing Sport Name", //Invalid sport
                "elena@uclm.es",
                new List<GameInvitationDTO>()
            );

            //Act
            var result = await controller.CreateGame(gameForCreate);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The specified sport does not exist.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateGame_InvalidResponsibleUser()
        {
            //Arrange
            var mock = new Mock<ILogger<GamesController>>();
            var controller = new GamesController(_context, mock.Object);

            var gameForCreate = new GameForCreateDTO(
                "Game With Bad User",
                new DateTime(2026, 11, 30),
                "Main Pavilion",
                "Friendly Match",
                "Football",
                "fakeuser@uclm.es", //invalid email
                new List<GameInvitationDTO>()
            );

            //Act
            var result = await controller.CreateGame(gameForCreate);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The responsible user does not exist.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateGame_InvalidTeamInvitation()
        {
            //Arrange
            var mock = new Mock<ILogger<GamesController>>();
            var controller = new GamesController(_context, mock.Object);

            var invalidInvitations = new List<GameInvitationDTO> 
            { 
                new GameInvitationDTO("Non-Existing Team", false, "Hello!") //invalid team
            };
            
            var gameForCreate = new GameForCreateDTO(
                "Game With Bad Team",
                new DateTime(2026, 11, 30),
                "Main Pavilion",
                "Friendly Match",
                "Football",
                "elena@uclm.es",
                invalidInvitations
            );

            //Act
            var result = await controller.CreateGame(gameForCreate);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The team 'Non-Existing Team' does not exist.", badRequestResult.Value);
        }
    }
}