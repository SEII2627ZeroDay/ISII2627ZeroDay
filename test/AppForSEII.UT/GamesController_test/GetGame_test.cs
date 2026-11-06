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
            var football = _context.Sports.FirstOrDefault(s => s.Name == "Football");
            if (football == null)
            {
                //Constructor parameters from Sport
                football = new Sport("Football", 11, 4, "Football match", "Standard rules");
                _context.Sports.Add(football);
                _context.SaveChanges();
            }

            var peter = _context.Users.FirstOrDefault(u => u.UserName == "peter@uclm.es") as ApplicationUser;
            if (peter == null)
            {
                //Constructor parameters from ApplicationUser
                peter = new ApplicationUser("peter-id", "Peter", "Jackson", "peter@uclm.es", new DateOnly(1998, 5, 20), 28, Gender.Male);
                _context.Users.Add(peter);
                _context.SaveChanges();
            }

            var elena = _context.Users.FirstOrDefault(u => u.UserName == "elena@uclm.es") as ApplicationUser;
            if (elena == null)
            {
                //Constructor parameters from ApplicationUser
                elena = new ApplicationUser("elena-id", "Elena", "Navarro", "elena@uclm.es", new DateOnly(1985, 3, 15), 41, Gender.Female);
                _context.Users.Add(elena);
                _context.SaveChanges();
            }

            var eaglesTeam = new Team("Eagles", "University football team", 15, 18, 35, peter!, football);
            _context.Teams.Add(eaglesTeam);

            var gameDate = new DateTime(2026, 11, 20, 18, 0, 0);
            var testGame = new Game("Champions Final", gameDate, "UCLM Stadium", "Final match of the tournament", football, elena!);
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