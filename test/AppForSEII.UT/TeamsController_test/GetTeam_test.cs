using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.TeamDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT.TeamsController_test
{
    public class GetTeam_test : AppForSEII4SqliteUT
    {
        private readonly TeamsController _controller;
        private readonly int _existingTeamId;
        private readonly int _teamWithoutInvitationsId;

        public GetTeam_test()
        {
            // Seed Sport
            var football = new Sport("Football", 11, 3, "11 vs 11 association football", "Rules");
            var basketball = new Sport("Basketball", 5, 2, "5 vs 5 basketball", "Rules");
            _context.Sports.AddRange(football, basketball);
            _context.SaveChanges();

            // Seed Users
            var captain = new ApplicationUser(
                "1",
                "Elena",
                "Navarro Martínez",
                "elena@uclm.es",
                new DateOnly(1985, 5, 20),
                41,
                Gender.Female)
            {
                EmailConfirmed = true
            };

            var player1 = new ApplicationUser(
                "2",
                "Lucas",
                "García López",
                "lucas@uclm.es",
                new DateOnly(2000, 3, 10),
                26,
                Gender.Male)
            {
                EmailConfirmed = true
            };

            var player2 = new ApplicationUser(
                "3",
                "Peter",
                "Jackson",
                "peter@uclm.es",
                new DateOnly(1998, 8, 15),
                28,
                Gender.Male)
            {
                EmailConfirmed = true
            };

            _context.ApplicationUsers.AddRange(captain, player1, player2);
            _context.SaveChanges();

            // Seed Team with invitations
            var eagles = new Team("Eagles", "University football team", 15, 18, 35, captain, football);
            // Seed Team without invitations
            var titans = new Team("Titans", "Basketball squad", 10, 18, 30, captain, basketball);

            _context.Teams.AddRange(eagles, titans);
            _context.SaveChanges();

            _existingTeamId = eagles.Id;
            _teamWithoutInvitationsId = titans.Id;

            // Seed Invitations for Eagles
            var invitation1 = new TeamInvitation(player1, eagles, "Welcome to the Eagles team!", true);
            var invitation2 = new TeamInvitation(player2, eagles, "Join us for next match", false);

            _context.TeamInvitations.AddRange(invitation1, invitation2);
            _context.SaveChanges();

            var mockLogger = new Mock<ILogger<TeamsController>>();
            _controller = new TeamsController(_context, mockLogger.Object);
        }

        [Fact]
        public async Task GetTeam_ExistingId_ReturnsOkResultWithTeamDetailDTO()
        {
            // Arrange
            var expectedPlayers = new List<TeamInvitationDTO>
            {
                new TeamInvitationDTO("2", "Lucas", "García López", 26, "Welcome to the Eagles team!", true),
                new TeamInvitationDTO("3", "Peter", "Jackson", 28, "Join us for next match", false)
            };

            var expectedTeam = new TeamDetailDTO(
                _existingTeamId,
                "Eagles",
                "University football team",
                15,
                18,
                35,
                "Football",
                "1",
                "Elena",
                "Navarro Martínez",
                41,
                expectedPlayers
            );

            // Act
            var result = await _controller.GetTeam(_existingTeamId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualTeam = Assert.IsType<TeamDetailDTO>(okResult.Value);
            Assert.Equal(expectedTeam, actualTeam);
        }

        [Fact]
        public async Task GetTeam_TeamWithoutInvitations_ReturnsOkWithEmptyPlayersList()
        {
            // Arrange
            var expectedTeam = new TeamDetailDTO(
                _teamWithoutInvitationsId,
                "Titans",
                "Basketball squad",
                10,
                18,
                30,
                "Basketball",
                "1",
                "Elena",
                "Navarro Martínez",
                41,
                new List<TeamInvitationDTO>()
            );

            // Act
            var result = await _controller.GetTeam(_teamWithoutInvitationsId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualTeam = Assert.IsType<TeamDetailDTO>(okResult.Value);
            Assert.Equal(expectedTeam, actualTeam);
            Assert.Empty(actualTeam.InvitedPlayers);
        }

        [Fact]
        public async Task GetTeam_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            int nonExistingId = 9999;

            // Act
            var result = await _controller.GetTeam(nonExistingId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
