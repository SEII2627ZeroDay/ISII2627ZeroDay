using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.RefereeDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT.RefereeGroupsController_test
{
    public class GetRefereeGroup_test : AppForSEII4SqliteUT
    {
        private readonly RefereeGroupsController _controller;
        private readonly int _existingGroupId;
        private readonly int _groupWithoutRefereesId;
        private readonly int _gameId;

        public GetRefereeGroup_test()
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
            _context.ApplicationUsers.Add(captain);
            _context.SaveChanges();

            // Seed Referees (the constructor of Referee receives rating, years refereeing and sportId)
            var referee1 = new Referee(5, 3, football.Id)
            {
                Id = "2",
                Name = "Lucas",
                Surname = "García López",
                Email = "lucas@uclm.es",
                EmailConfirmed = true
            };
            var referee2 = new Referee(4, 2, football.Id)
            {
                Id = "3",
                Name = "Peter",
                Surname = "Jackson",
                Email = "peter@uclm.es",
                EmailConfirmed = true
            };
            _context.Referee.AddRange(referee1, referee2);
            _context.SaveChanges();

            // Seed Games (ADAPT to the constructor/properties of your Game model)
            // if Game has a foreign key property (the name may differ):
            var finalGame = new Game { Name = "Final", Date = DateTime.Now.AddDays(10), Place = "Stadium Seville", SportId = football.Id, ResponsibleForId = captain.Id };
            var friendlyGame = new Game { Name = "Friendly", Date = DateTime.Now.AddDays(20), Place = "Sports Hall Albacete", SportId = basketball.Id, ResponsibleForId = captain.Id };
            _context.Games.AddRange(finalGame, friendlyGame);
            _context.SaveChanges();
            _gameId = finalGame.Id;

            // Seed RefereeGroup with referees assigned
            var group1 = new RefereeGroup { Name = "Group1", Description = "Main group", Rules = "Standard rules", GameId = finalGame.Id };
            // Seed RefereeGroup without referees assigned
            var group2 = new RefereeGroup { Name = "Group2", Description = "Empty group", Rules = "No rules", GameId = friendlyGame.Id };
            _context.RefereeGroup.AddRange(group1, group2);
            _context.SaveChanges();

            _existingGroupId = group1.Id;
            _groupWithoutRefereesId = group2.Id;

            // Seed RefereeAssignedTo for Group1
            var assignment1 = new RefereeAssignedTo { RefereeGroupId = group1.Id, RefereeId = referee1.Id, AcceptedAssignement = false, Role = "Main referee", RoleDescription = "Leads the match" };
            var assignment2 = new RefereeAssignedTo { RefereeGroupId = group1.Id, RefereeId = referee2.Id, AcceptedAssignement = false, Role = "Assistant", RoleDescription = "Helps the main referee" };
            _context.RefereeAssignedTo.AddRange(assignment1, assignment2);
            _context.SaveChanges();

            var mockLogger = new Mock<ILogger<RefereeGroupsController>>();
            _controller = new RefereeGroupsController(_context, mockLogger.Object);
        }

        [Fact]
        public async Task GetRefereeGroup_ExistingId_ReturnsOkResultWithRefereeGroupDTO()
        {
            // Arrange
            var expectedAssignments = new List<RefereeAssignedToDTO>
            {
                new RefereeAssignedToDTO(_existingGroupId, "2", false, "Main referee", "Leads the match"),
                new RefereeAssignedToDTO(_existingGroupId, "3", false, "Assistant", "Helps the main referee")
            };

            var expectedGroup = new RefereeGroupDTO(
                _existingGroupId,
                "Group1",
                "Main group",
                "Standard rules",
                _gameId,
                expectedAssignments
            );

            // Act
            var result = await _controller.GetRefereeGroup(_existingGroupId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualGroup = Assert.IsType<RefereeGroupDTO>(okResult.Value);
            Assert.Equal(expectedGroup, actualGroup);
        }

        [Fact]
        public async Task GetRefereeGroup_GroupWithoutReferees_ReturnsOkWithEmptyAssignmentsList()
        {
            // Act
            var result = await _controller.GetRefereeGroup(_groupWithoutRefereesId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualGroup = Assert.IsType<RefereeGroupDTO>(okResult.Value);
            Assert.Equal("Group2", actualGroup.Name);
            Assert.Empty(actualGroup.RefereeAssignedTos);
        }

        [Fact]
        public async Task GetRefereeGroup_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            int nonExistingId = 9999;

            // Act
            var result = await _controller.GetRefereeGroup(nonExistingId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}