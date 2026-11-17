using System.Runtime.CompilerServices;
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.TeamDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq; // Agregado para usar Mock
using Xunit; // Requerido para [Fact], [Theory], Assert

namespace AppForSEII.UT.TeamsController_test
{
    public class GetTeamsForGame_test: AppForSEII4SqliteUT
    {
        public GetTeamsForGame_test()
        {
            //Seed Sports (Usando el constructor exacto del proyecto)
            var football = new Sport("Football", 11, 3, "11 vs 11 football", "Rules");
            var basketball = new Sport("Basketball", 5, 2, "5 vs 5 basketball", "Rules");
            _context.Sports.AddRange(football, basketball);
            _context.SaveChanges();

            //Seed Users (Usando el constructor exacto del proyecto)
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

            _context.ApplicationUsers.AddRange(elena, peter);
            _context.SaveChanges();

            //seed teams
            var teams = new List<Team>
            {
                new Team("Titans", "Basketball squad", 10, 18, 30, elena, basketball),
                new Team("Eagles", "University football team", 15, 18, 35, peter, football)
            };
            _context.Teams.AddRange(teams);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetTeamsForGameNULL_test()
        {
            List<TeamDetailDTO> expectedTeams = new List<TeamDetailDTO>()
            {
                new TeamDetailDTO(1, "Titans", "Basketball squad", 10, 18, 30, "Basketball", "1", "Elena", "Navarro Martínez", 41, new List<TeamInvitationDTO>()),
                new TeamDetailDTO(2, "Eagles", "University football team", 15, 18, 35, "Football", "3", "Peter", "Jackson", 28, new List<TeamInvitationDTO>()),
            };

            var mock = new Mock<ILogger<TeamsController>>();
            ILogger<TeamsController> logger = mock.Object;
            TeamsController controller = new TeamsController(_context, logger);

            //act
            var result = await controller.GetTeamsForGame(null, null);

            //assert
            var okresult = Assert.IsType<OkObjectResult>(result);
            var teamsactualresult = Assert.IsType<List<TeamDetailDTO>>(okresult.Value);
            Assert.Equal(expectedTeams, teamsactualresult);
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetTeamsForGame))]
        public async Task GetTeamsForGameFilter_test(string? name, string? Sports, List<TeamDetailDTO> expectedTeams)
        {
            var mock = new Mock<ILogger<TeamsController>>();
            ILogger<TeamsController> logger = mock.Object;
            TeamsController controller = new TeamsController(_context, logger);

            //act
            var result = await controller.GetTeamsForGame(name, Sports);

            //assert
            var okresult = Assert.IsType<OkObjectResult>(result);
            var teamsactualresult = Assert.IsType<List<TeamDetailDTO>>(okresult.Value);
            Assert.Equal(expectedTeams, teamsactualresult);
        }

        public static IEnumerable<object?[]> TestCasesFor_GetTeamsForGame()
        {
            List<TeamDetailDTO> TeamsDTOs = new List<TeamDetailDTO>()
            {
                new TeamDetailDTO(1, "Titans", "Basketball squad", 10, 18, 30, "Basketball", "1", "Elena", "Navarro Martínez", 41, new List<TeamInvitationDTO>()),
                new TeamDetailDTO(2, "Eagles", "University football team", 15, 18, 35, "Football", "3", "Peter", "Jackson", 28, new List<TeamInvitationDTO>()),
            };
            var TC1WithoutFilters = new List<TeamDetailDTO> {TeamsDTOs[0], TeamsDTOs[1]};
            var TC2TitleFilter = new List<TeamDetailDTO> {TeamsDTOs[0]};
            var TC3SportFilter = new List<TeamDetailDTO> {TeamsDTOs[0]};

            var allTests = new List<object?[]>
            {
                new object?[] {null, null, TC1WithoutFilters},
                new object?[]{"Titans", null, TC2TitleFilter},
                new object?[]{null, "Basketball", TC3SportFilter}
            };
            
            return allTests;
        }
    }
}