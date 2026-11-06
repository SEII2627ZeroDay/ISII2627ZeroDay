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
            //Seed de datos base controlados e inmunes a nulos
            var football = _context.Sports.FirstOrDefault(s => s.Name == "Football") ?? new Sport { Name = "Football" };
            var peter = _context.Users.FirstOrDefault(u => u.UserName == "peter@uclm.es") ?? new ApplicationUser { UserName = "peter@uclm.es", Name = "Peter", Surname = "Jackson", Age = 28 };
            var elena = _context.Users.FirstOrDefault(u => u.UserName == "elena@uclm.es") ?? new ApplicationUser { UserName = "elena@uclm.es", Name = "Elena", Surname = "Navarro", Age = 41 };

            if (_context.Entry(football).State == Microsoft.EntityFrameworkCore.EntityState.Detached) _context.Sports.Add(football);
            if (_context.Entry(peter).State == Microsoft.EntityFrameworkCore.EntityState.Detached) _context.Users.Add(peter);
            if (_context.Entry(elena).State == Microsoft.EntityFrameworkCore.EntityState.Detached) _context.Users.Add(elena);
            _context.SaveChanges();

            //Guardamos un equipo semilla para las pruebas de invitaciones válidas
            var eaglesTeam = new Team("Eagles", "University football team", 15, 18, 35, peter, football);
            _context.Teams.Add(eaglesTeam);

            //Guardamos un partido existente para comprobar la validación de unicidad de nombres
            var existingGame = new Game("Duplicate Game Name", new DateTime(2026, 11, 25), "Stadium", "Desc", football, elena);
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
                "Non-Existing Sport Name", // Deporte inválido
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
                "fakeuser@uclm.es", // Correo inválido
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
                new GameInvitationDTO("Non-Existing Team", false, "Hello!") // Equipo inválido
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