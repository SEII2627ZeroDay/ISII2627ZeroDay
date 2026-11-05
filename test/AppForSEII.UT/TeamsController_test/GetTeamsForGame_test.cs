using System.Runtime.CompilerServices;
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.TeamDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.UT.TeamsController_test
{
    public class GetTeamsForGame_test: AppForSEII4SqliteUT
    {
        public GetTeamsForGame_test()
        {
            var football = _context.Sports.FirstOrDefault(s => s.Name == "Football");
            var basketball = _context.Sports.FirstOrDefault(s => s.Name == "Basketball");
            var peter = _context.Users.FirstOrDefault(u => u.UserName == "peter@uclm.es");
            var elena = _context.Users.FirstOrDefault(u => u.UserName == "elena@uclm.es");

            var teams = new List<Team>();

            if (football != null && peter != null)
            {
                teams.Add(new Team("Eagles", "University football team", 15, 18, 35, peter, football));
            }

            if (basketball != null && elena != null)
            {
                teams.Add(new Team("Titans", "Basketball squad", 10, 18, 30, elena, basketball));
            }

            _context.Teams.AddRange(teams);
            _context.SaveChanges();
        }

        public async Task GetTeamsForGameNULL_test()
        {
            List<TeamDetailDTO> expectedTeams = new List<TeamDetailDTO>()
            {
                new TeamDetailDTO(1, "Titans", "Basketball squad", 10, 18, 30, "Basketball", "Elena", "Navarro Martínez", 41, new List<TeamPlayerDTO>()),
                new TeamDetailDTO(2, "Eagles", "University football team", 15, 18, 35, "Football", "Peter", "Jackson", 28, new List<TeamPlayerDTO>()),
            };

            var mock = new Mock<ILogger<TeamsController>>();
            ILogger<TeamsController> logger = mock.Object;
            TeamsController controller = new TeamsController(_context, logger);

            //act
            var result = await controller.GetTeam(null);

            //assert
            var okresult = Assert.IsType<OkObjectResult>(result);
            var teamsactualresult = Assert.IsType<List<TeamDetailDTO>>(okresult.Value);
            Assert.Equal(expectedTeams, teamsactualresult);
        }
    }
}