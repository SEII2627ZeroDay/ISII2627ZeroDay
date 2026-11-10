using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.TeamDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT.TeamsController_test
{
    public class CreateTeam_test : AppForSEII4SqliteUT
    {
        private readonly TeamsController _controller;

        public CreateTeam_test()
        {
            // Seed Sports
            var tennis = new Sport("Tennis", 2, 1, "Singles/doubles tennis match", "Rules");
            var football = new Sport("Football", 11, 3, "11 vs 11 association football", "Rules");
            var basketball = new Sport("Basketball", 5, 2, "5 vs 5 basketball on standard court", "Rules");
            _context.Sports.AddRange(tennis, football, basketball);
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

            var sofia = new ApplicationUser(
                "4",
                "Sofía",
                "Fernández Ruiz",
                "sofia@uclm.es",
                new DateOnly(2010, 11, 25),
                16,
                Gender.Female)
            {
                EmailConfirmed = true
            };

            _context.ApplicationUsers.AddRange(elena, lucas, peter, sofia);
            _context.SaveChanges();

            // Seed an existing team to test duplicate name (Alternative Flow 5)
            var existingTeam = new Team("Eagles", "Existing tennis squad", 10, 18, 50, elena, tennis);
            _context.Teams.Add(existingTeam);
            _context.SaveChanges();

            var mockLogger = new Mock<ILogger<TeamsController>>();
            _controller = new TeamsController(_context, mockLogger.Object);
        }

        [Fact]
        public async Task CreateTeam_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new TeamForCreateDTO(
                name: "Hawks",
                description: "A competitive tennis team",
                maxMembers: 5,
                minAge: 20,
                maxAge: 45,
                sport: "Tennis",
                captainId: "1", // Elena (age 41)
                invitedPlayers: new List<TeamInvitationDTO>
                {
                    new TeamInvitationDTO("2", "Dear Player, welcome to the team, Lucas!") // Lucas (age 26)
                }
            );

            // Act
            var result = await _controller.CreateTeam(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualTeam = Assert.IsType<TeamDetailDTO>(createdResult.Value);

            var expectedTeam = new TeamDetailDTO(
                actualTeam.Id,
                "Hawks",
                "A competitive tennis team",
                5,
                20,
                45,
                "Tennis",
                "Elena",
                "Navarro Martínez",
                41,
                new List<TeamPlayerDTO>
                {
                    new TeamPlayerDTO("Lucas", "García López", 26, "Dear Player, welcome to the team, Lucas!", false)
                }
            );

            Assert.Equal(expectedTeam, actualTeam);

            // Verify team and pending invitation in database
            var teamInDb = await _context.Teams.Include(t => t.TeamInvitations).FirstOrDefaultAsync(t => t.Name == "Hawks");
            Assert.NotNull(teamInDb);
            Assert.Single(teamInDb.TeamInvitations);
            Assert.False(teamInDb.TeamInvitations.First().InvitationAccepted);
        }

        [Fact]
        public async Task CreateTeam_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var dto = new TeamForCreateDTO("", null, 0, 0, 0, "", "", new List<TeamInvitationDTO>());
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.CreateTeam(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_CreateTeam_Errors))]
        public async Task CreateTeam_ValidationErrors_Theory(TeamForCreateDTO dto, Type expectedResultType, string expectedErrorSubstring)
        {
            // Arrange (provided via MemberData)

            // Act
            var result = await _controller.CreateTeam(dto);

            // Assert
            Assert.IsType(expectedResultType, result);
            var objectResult = result as ObjectResult;
            Assert.NotNull(objectResult);
            if (!string.IsNullOrEmpty(expectedErrorSubstring))
            {
                var message = objectResult.Value as string;
                Assert.NotNull(message);
                Assert.Contains(expectedErrorSubstring, message);
            }
        }

        public static IEnumerable<object[]> TestCasesFor_CreateTeam_Errors()
        {
            // Helper to build valid baseline or modified versions using the parameterized constructor
            static TeamForCreateDTO CreateDTO(
                string name = "Tigers",
                string? description = "Baseline team description",
                int maxMembers = 5,
                int minAge = 20,
                int maxAge = 45,
                string sport = "Tennis",
                string captainId = "1",
                IList<TeamInvitationDTO>? invitedPlayers = null)
            {
                return new TeamForCreateDTO(
                    name,
                    description,
                    maxMembers,
                    minAge,
                    maxAge,
                    sport,
                    captainId,
                    invitedPlayers ?? new List<TeamInvitationDTO> { new TeamInvitationDTO("2", "Dear Player, welcome to the Tigers!") }
                );
            }

            // TC1: MinAge > MaxAge
            yield return new object[] { CreateDTO(minAge: 40, maxAge: 25), typeof(BadRequestObjectResult), "Minimum age cannot be greater than maximum age." };

            // TC2: Duplicate team name (Alternative Flow 5 - "Eagles" already exists)
            yield return new object[] { CreateDTO(name: "Eagles"), typeof(ConflictObjectResult), "already exists" };

            // TC3: Sport does not exist
            yield return new object[] { CreateDTO(sport: "Curling"), typeof(NotFoundObjectResult), "was not found" };

            // TC4: Captain does not exist
            yield return new object[] { CreateDTO(captainId: "999"), typeof(NotFoundObjectResult), "was not found" };

            // TC5: Captain age outside range (Elena is 41, range is 18-35)
            yield return new object[] { CreateDTO(minAge: 18, maxAge: 35), typeof(BadRequestObjectResult), "outside the team age range" };

            // TC6: Alternative Flow 4: Number of players is too small for selected sport (Football requires min 11, totalMembers = 2)
            yield return new object[] { CreateDTO(sport: "Football"), typeof(BadRequestObjectResult), "too small for Football" };

            // TC7: Total members exceeds MaxMembers (maxMembers = 2, total = 3: captain + 2 invited)
            yield return new object[] {
                CreateDTO(
                    maxMembers: 2,
                    invitedPlayers: new List<TeamInvitationDTO>
                    {
                        new TeamInvitationDTO("2", "Join Tigers player 1"),
                        new TeamInvitationDTO("3", "Join Tigers player 2")
                    }),
                typeof(BadRequestObjectResult),
                "exceeds maximum allowed members"
            };

            // TC8: Duplicate invited players (Lucas "2" listed twice)
            yield return new object[] {
                CreateDTO(
                    invitedPlayers: new List<TeamInvitationDTO>
                    {
                        new TeamInvitationDTO("2", "Invitation 1"),
                        new TeamInvitationDTO("2", "Invitation 2")
                    }),
                typeof(BadRequestObjectResult),
                "Duplicate invited players"
            };

            // TC9: Captain cannot invite themselves (Elena "1" listed in invited players)
            yield return new object[] {
                CreateDTO(
                    invitedPlayers: new List<TeamInvitationDTO>
                    {
                        new TeamInvitationDTO("1", "Captain self invitation")
                    }),
                typeof(BadRequestObjectResult),
                "Captain cannot be invited"
            };

            // TC10: Invited player does not exist ("unknown_user")
            yield return new object[] {
                CreateDTO(
                    invitedPlayers: new List<TeamInvitationDTO>
                    {
                        new TeamInvitationDTO("unknown_user", "Invitation message")
                    }),
                typeof(NotFoundObjectResult),
                "do not exist"
            };

            // TC11: Invited player age outside range (Sofia "4" is 16, range is 20-45)
            yield return new object[] {
                CreateDTO(
                    invitedPlayers: new List<TeamInvitationDTO>
                    {
                        new TeamInvitationDTO("4", "Dear Player, welcome Sofia")
                    }),
                typeof(BadRequestObjectResult),
                "does not meet the age criteria"
            };

            // TC12: Invitation message does not start with "Dear Player,"
            yield return new object[] {
                CreateDTO(
                    invitedPlayers: new List<TeamInvitationDTO>
                    {
                        new TeamInvitationDTO("2", "Welcome Lucas!")
                    }),
                typeof(BadRequestObjectResult),
                "Error! One of the invitation messages has not been properly formatted"
            };

            // TC13: Multiple players where one message is valid and another is empty
            yield return new object[] {
                CreateDTO(
                    invitedPlayers: new List<TeamInvitationDTO>
                    {
                        new TeamInvitationDTO("2", "Dear Player, welcome to the Tigers!"),
                        new TeamInvitationDTO("3", "")
                    }),
                typeof(BadRequestObjectResult),
                "Error! One of the invitation messages has not been properly formatted"
            };
        }
    }
}
