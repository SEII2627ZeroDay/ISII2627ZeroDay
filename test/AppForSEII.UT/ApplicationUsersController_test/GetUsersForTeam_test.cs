using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ApplicationUserDTO;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT.ApplicationUsersController_test
{
    public class GetUsersForTeam_test : AppForSEII4SqliteUT
    {
        private readonly ApplicationUsersController _controller;

        public GetUsersForTeam_test()
        {
            // Seed Sports
            var football = new Sport("Football", 11, 3, "11 vs 11 football", "Rules");
            var basketball = new Sport("Basketball", 5, 2, "5 vs 5 basketball", "Rules");
            _context.Sports.AddRange(football, basketball);
            _context.SaveChanges();

            // Seed Users
            var elena = new ApplicationUser(
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

            var lucas = new ApplicationUser(
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

            var peter = new ApplicationUser(
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

            _context.ApplicationUsers.AddRange(elena, lucas, peter);
            _context.SaveChanges();

            // Seed Interests
            var interestElena = new InterestedIn(basketball, elena, 4);
            var interestLucas = new InterestedIn(football, lucas, 5);
            var interestPeter = new InterestedIn(football, peter, 4);

            _context.InterestedIns.AddRange(interestElena, interestLucas, interestPeter);
            _context.SaveChanges();

            var mockLogger = new Mock<ILogger<ApplicationUsersController>>();
            _controller = new ApplicationUsersController(_context, mockLogger.Object);
        }

        [Fact]
        public async Task GetUsersForTeam_NullFilters_ReturnsAllUsersOrderedBySurnameThenName()
        {
            // Arrange
            var expectedUsers = new List<ApplicationUserForTeamDTO>
            {
                new ApplicationUserForTeamDTO("2", "Lucas", "García López", 26, "Football", 5),
                new ApplicationUserForTeamDTO("3", "Peter", "Jackson", 28, "Football", 4),
                new ApplicationUserForTeamDTO("1", "Elena", "Navarro Martínez", 41, "Basketball", 4)
            };

            // Act
            var result = await _controller.GetUsersForTeam(null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualUsers = Assert.IsType<List<ApplicationUserForTeamDTO>>(okResult.Value);
            Assert.Equal(expectedUsers, actualUsers);
        }

        [Fact]
        public async Task GetUsersForTeam_FilterBySport_ReturnsFilteredUsers()
        {
            // Arrange
            var expectedUsers = new List<ApplicationUserForTeamDTO>
            {
                new ApplicationUserForTeamDTO("2", "Lucas", "García López", 26, "Football", 5),
                new ApplicationUserForTeamDTO("3", "Peter", "Jackson", 28, "Football", 4)
            };

            // Act
            var result = await _controller.GetUsersForTeam("Football", null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualUsers = Assert.IsType<List<ApplicationUserForTeamDTO>>(okResult.Value);
            Assert.Equal(expectedUsers, actualUsers);
        }

        [Fact]
        public async Task GetUsersForTeam_FilterBySkill_ReturnsFilteredUsers()
        {
            // Arrange
            var expectedUsers = new List<ApplicationUserForTeamDTO>
            {
                new ApplicationUserForTeamDTO("3", "Peter", "Jackson", 28, "Football", 4),
                new ApplicationUserForTeamDTO("1", "Elena", "Navarro Martínez", 41, "Basketball", 4)
            };

            // Act
            var result = await _controller.GetUsersForTeam(null, 4);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualUsers = Assert.IsType<List<ApplicationUserForTeamDTO>>(okResult.Value);
            Assert.Equal(expectedUsers, actualUsers);
        }

        [Fact]
        public async Task GetUsersForTeam_FilterBySportAndSkill_ReturnsMatchingUser()
        {
            // Arrange
            var expectedUsers = new List<ApplicationUserForTeamDTO>
            {
                new ApplicationUserForTeamDTO("2", "Lucas", "García López", 26, "Football", 5)
            };

            // Act
            var result = await _controller.GetUsersForTeam("Football", 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualUsers = Assert.IsType<List<ApplicationUserForTeamDTO>>(okResult.Value);
            Assert.Equal(expectedUsers, actualUsers);
        }

        [Fact]
        public async Task GetUsersForTeam_NoMatchingUsers_ReturnsEmptyList()
        {
            // Arrange - sport not played by anyone with skill 1
            // Act
            var result = await _controller.GetUsersForTeam("Tennis", 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualUsers = Assert.IsType<List<ApplicationUserForTeamDTO>>(okResult.Value);
            Assert.Empty(actualUsers);
        }
    }
}