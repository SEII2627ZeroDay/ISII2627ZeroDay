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
        private readonly int _existingGroupId;
        private readonly int _groupWithoutRefereesId;
        private readonly int _gameId;
        private readonly int _friendlyGameId;

        public GetRefereeGroup_test()
        {
            var football = new Sport("Football", 11, 3, "11 vs 11 association football", "Rules");
            var basketball = new Sport("Basketball", 5, 2, "5 vs 5 basketball", "Rules");
            _context.Sports.AddRange(football, basketball);
            _context.SaveChanges();

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

            var referee1 = new Referee(5, 3, football.Id) { Id = "2", Name = "Lucas", Surname = "García López" };
            var referee2 = new Referee(4, 2, football.Id) { Id = "3", Name = "Peter", Surname = "Jackson" };
            _context.Referee.AddRange(referee1, referee2);
            _context.SaveChanges();

            // TODO: replace both Game objects with the constructor of Game that initializes the properties
            var finalGame = new Game { Name = "Final", Date = DateTime.Now.AddDays(10), Place = "Stadium Seville", SportId = football.Id, ResponsibleForId = captain.Id };
            var friendlyGame = new Game { Name = "Friendly", Date = DateTime.Now.AddDays(20), Place = "Sports Hall Albacete", SportId = basketball.Id, ResponsibleForId = captain.Id };
            _context.Games.AddRange(finalGame, friendlyGame);
            _context.SaveChanges();
            _gameId = finalGame.Id;
            _friendlyGameId = friendlyGame.Id;

            // group with referees assigned
            var group1 = new RefereeGroup("Group1", "Main group", "Standard rules", finalGame.Id);
            // group without referees assigned
            var group2 = new RefereeGroup("Group2", "Empty group", "No rules", friendlyGame.Id);
            _context.RefereeGroup.AddRange(group1, group2);
            _context.SaveChanges();
            _existingGroupId = group1.Id;
            _groupWithoutRefereesId = group2.Id;

            _context.RefereeAssignedTo.AddRange(
                new RefereeAssignedTo(group1.Id, referee1.Id, false, "Main referee", "Leads the match"),
                new RefereeAssignedTo(group1.Id, referee2.Id, false, "Assistant", "Helps the main referee"));
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRefereeGroup_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<RefereeGroupsController>>();
            ILogger<RefereeGroupsController> logger = mock.Object;
            var controller = new RefereeGroupsController(_context, logger);

            var expectedGroup = new RefereeGroupDTO(_existingGroupId, "Group1", "Main group", "Standard rules", _gameId,
                new List<RefereeAssignedToDTO>
                {
                    new RefereeAssignedToDTO(_existingGroupId, "2", false, "Main referee", "Leads the match"),
                    new RefereeAssignedToDTO(_existingGroupId, "3", false, "Assistant", "Helps the main referee")
                });

            // Act
            var result = await controller.GetRefereeGroup(_existingGroupId);

            // Assert
            // we check that the response type is OK and obtain the referee group
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualGroup = Assert.IsType<RefereeGroupDTO>(okResult.Value);
            // we check that the expected and actual are the same
            Assert.Equal(expectedGroup, actualGroup);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRefereeGroup_WithoutReferees_test()
        {
            // Arrange
            var mock = new Mock<ILogger<RefereeGroupsController>>();
            ILogger<RefereeGroupsController> logger = mock.Object;
            var controller = new RefereeGroupsController(_context, logger);

            var expectedGroup = new RefereeGroupDTO(_groupWithoutRefereesId, "Group2", "Empty group", "No rules", _friendlyGameId,
                new List<RefereeAssignedToDTO>());

            // Act
            var result = await controller.GetRefereeGroup(_groupWithoutRefereesId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualGroup = Assert.IsType<RefereeGroupDTO>(okResult.Value);
            Assert.Equal(expectedGroup, actualGroup);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRefereeGroup_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<RefereeGroupsController>>();
            ILogger<RefereeGroupsController> logger = mock.Object;
            var controller = new RefereeGroupsController(_context, logger);

            // Act
            var result = await controller.GetRefereeGroup(0);

            // Assert
            // we check that the response type is NotFound
            Assert.IsType<NotFoundResult>(result);
        }
    }
}