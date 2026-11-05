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
                    new TeamInvitationDTO("2", "Welcome to the team, Lucas!") // Lucas (age 26)
                }
            );

            // Act
            var result = await _controller.CreateTeam(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualTeam = Assert.IsType<TeamDetailDTO>(createdResult.Value);

            Assert.Equal("Hawks", actualTeam.Name);
            Assert.Equal("Tennis", actualTeam.Sport);
            Assert.Equal("Elena", actualTeam.CaptainName);
            Assert.Equal("Navarro Martínez", actualTeam.CaptainSurname);
            Assert.Equal(41, actualTeam.CaptainAge);
            Assert.Single(actualTeam.InvitedPlayers);
            Assert.Equal("Lucas", actualTeam.InvitedPlayers[0].Name);
            Assert.Equal("García López", actualTeam.InvitedPlayers[0].Surname);
            Assert.Equal(26, actualTeam.InvitedPlayers[0].Age);
            Assert.Equal("Welcome to the team, Lucas!", actualTeam.InvitedPlayers[0].InvitationMessage);
            Assert.False(actualTeam.InvitedPlayers[0].InvitationAccepted);

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
            var dto = new TeamForCreateDTO();
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
            // Valid baseline: Name="Tigers", Sport="Tennis" (min 2), Captain="1" (age 41), MinAge=20, MaxAge=45, MaxMembers=5, Invited=["2" (age 26)]
            Func<TeamForCreateDTO> validBaseline = () => new TeamForCreateDTO(
                "Tigers",
                "Baseline team description",
                5,
                20,
                45,
                "Tennis",
                "1",
                new List<TeamInvitationDTO> { new TeamInvitationDTO("2", "Welcome to the Tigers!") }
            );

            // TC1: MinAge > MaxAge
            var tc1 = validBaseline();
            tc1.MinAge = 40;
            tc1.MaxAge = 25;
            yield return new object[] { tc1, typeof(BadRequestObjectResult), "Minimum age cannot be greater than maximum age." };

            // TC2: Duplicate team name (Alternative Flow 5 - "Eagles" already exists)
            var tc2 = validBaseline();
            tc2.Name = "Eagles";
            yield return new object[] { tc2, typeof(ConflictObjectResult), "already exists" };

            // TC3: Sport does not exist
            var tc3 = validBaseline();
            tc3.Sport = "Curling";
            yield return new object[] { tc3, typeof(NotFoundObjectResult), "was not found" };

            // TC4: Captain does not exist
            var tc4 = validBaseline();
            tc4.CaptainId = "999";
            yield return new object[] { tc4, typeof(NotFoundObjectResult), "was not found" };

            // TC5: Captain age outside range (Elena is 41, range is 18-35)
            var tc5 = validBaseline();
            tc5.MinAge = 18;
            tc5.MaxAge = 35;
            yield return new object[] { tc5, typeof(BadRequestObjectResult), "outside the team age range" };

            // TC6: Alternative Flow 4: Number of players is too small for selected sport (Football requires min 11, totalMembers = 2)
            var tc6 = validBaseline();
            tc6.Sport = "Football";
            yield return new object[] { tc6, typeof(BadRequestObjectResult), "too small for Football" };

            // TC7: Total members exceeds MaxMembers (maxMembers = 2, total = 3: captain + 2 invited)
            var tc7 = validBaseline();
            tc7.MaxMembers = 2;
            tc7.InvitedPlayers = new List<TeamInvitationDTO>
            {
                new TeamInvitationDTO("2", "Join Tigers player 1"),
                new TeamInvitationDTO("3", "Join Tigers player 2")
            };
            yield return new object[] { tc7, typeof(BadRequestObjectResult), "exceeds maximum allowed members" };

            // TC8: Duplicate invited players (Lucas "2" listed twice)
            var tc8 = validBaseline();
            tc8.InvitedPlayers = new List<TeamInvitationDTO>
            {
                new TeamInvitationDTO("2", "Invitation 1"),
                new TeamInvitationDTO("2", "Invitation 2")
            };
            yield return new object[] { tc8, typeof(BadRequestObjectResult), "Duplicate invited players" };

            // TC9: Captain cannot invite themselves (Elena "1" listed in invited players)
            var tc9 = validBaseline();
            tc9.InvitedPlayers = new List<TeamInvitationDTO>
            {
                new TeamInvitationDTO("1", "Captain self invitation")
            };
            yield return new object[] { tc9, typeof(BadRequestObjectResult), "Captain cannot be invited" };

            // TC10: Invited player does not exist ("unknown_user")
            var tc10 = validBaseline();
            tc10.InvitedPlayers = new List<TeamInvitationDTO>
            {
                new TeamInvitationDTO("unknown_user", "Invitation message")
            };
            yield return new object[] { tc10, typeof(NotFoundObjectResult), "do not exist" };

            // TC11: Invited player age outside range (Sofia "4" is 16, range is 20-45)
            var tc11 = validBaseline();
            tc11.InvitedPlayers = new List<TeamInvitationDTO>
            {
                new TeamInvitationDTO("4", "Welcome Sofia")
            };
            yield return new object[] { tc11, typeof(BadRequestObjectResult), "does not meet the age criteria" };
        }
    }
}
