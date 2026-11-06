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
    public class GetGame_test : AppForSEII4SqliteUT
    {
        private int _seededGameId;
        public GetGame_test()
        {
            //SeedData
            var football = _context.Sports.FirstOrDefault(s => s.Name == "Football") ?? new Sport {Name="Football"};
            var peter = _context.Users.FirstOrDefault(u => u.UserName == "peter@uclm.es") ?? new ApplicationUser {UserName = "peter@uclm.es", Name = "Peter", Surname = "Jackson", Age = 28};
            var elena = _context.Users.FirstOrDefault(u => u.UserName == "elena@uclm.es") ?? new ApplicationUser {UserName = "elena@uclm.es", Name = "Elena", Surname = "Navarro", Age = 41};

            if (_context.Entry(football).State == Microsoft.EntityFrameworkCore.EntityState.Detached) _context.Sports.Add(football);
            if (_context.Entry(peter).State == Microsoft.EntityFrameworkCore.EntityState.Detached) _context.Users.Add(peter);
            if (_context.Entry(elena).State == Microsoft.EntityFrameworkCore.EntityState.Detached) _context.Users.Add(elena);
            _context.SaveChanges();

            var eaglesTeam = new Team("Eagles", "University football team", 15, 18, 35, peter, football);
            _context.Teams.Add(eaglesTeam);

            var gameDate = new DateTime(2026, 11, 20, 18, 0, 0);
            var testGame = new Game("Champions Final", gameDate, "UCLM Stadium", "Final match of the tournament", football, elena);
            _context.Games.Add(testGame);
            
            _context.SaveChanges();

            _seededGameId = testGame.Id;

            var invitation = new GameInvitation(eaglesTeam, testGame, false, "Te invitamos a jugar la gran final");
            _context.GameInvitations.Add(invitation);
            
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetGame_ExistingId()
        {
            //Arrange
            var mock = new Mock<ILogger<GamesController>>();
            ILogger<GamesController> logger = mock.Object;
            GamesController controller = new GamesController(_context, logger);

            var expectedInvitations = new List<GameInvitationDTO>
            {
                new GameInvitationDTO("Eagles", false, "Te invitamos a jugar la gran final")
            };
            var expectedGameDto = new GameDetailDTO(
                _seededGameId, 
                "Champions Final", 
                new DateTime(2026, 11, 20, 18, 0, 0), 
                "UCLM Stadium", 
                "Final match of the tournament", 
                "Football", 
                "elena@uclm.es", 
                expectedInvitations
            );

            //Act
            var result = await controller.GetGame(_seededGameId);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualGameDto = Assert.IsType<GameDetailDTO>(okResult.Value);
            Assert.Equal(expectedGameDto, actualGameDto); 
        }

        [Fact]
        public async Task GetGame_NonExistingId()
        {
            //Arrange
            var mock = new Mock<ILogger<GamesController>>();
            ILogger<GamesController> logger = mock.Object;
            GamesController controller = new GamesController(_context, logger);

            int nonExistingId = _seededGameId+999;

            //Act
            var result = await controller.GetGame(nonExistingId);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}