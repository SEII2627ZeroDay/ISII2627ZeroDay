using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.TeamDTOs;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(ApplicationDbContext context, ILogger<TeamsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Slide 11 example: ComputeDivision
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ComputeDivision(decimal op1, decimal op2)
        {
            if (op2 == 0)
            {
                _logger.LogError($"{DateTime.Now} Exception: op2=0, division by 0");
                return BadRequest("op2 must be different from 0");
            }
            decimal result = decimal.Round(op1 / op2, 2);
            return Ok(result);
        }

        // GET: api/Teams/GetTeam?id=1
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(TeamDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetTeam(int id)
        {
            var team = await _context.Teams
                .Include(t => t.Sport)
                .Include(t => t.Captain)
                .Include(t => t.TeamInvitations)
                    .ThenInclude(ti => ti.User)
                .Where(t => t.Id == id)
                .Select(t => new TeamDetailDTO(
                    t.Id,
                    t.Name,
                    t.Description,
                    t.MaxMembers,
                    t.MinAge,
                    t.MaxAge,
                    t.Sport.Name,
                    t.Captain.Name ?? "",
                    t.Captain.Surname ?? "",
                    t.Captain.Age,
                    t.TeamInvitations.Select(ti => new TeamPlayerDTO(
                        ti.User.Name ?? "",
                        ti.User.Surname ?? "",
                        ti.User.Age,
                        ti.InvitationMessage,
                        ti.InvitationAccepted
                    )).ToList()
                ))
                .FirstOrDefaultAsync();

            if (team == null)
            {
                _logger.LogError($"Team with id {id} was not found");
                return NotFound();
            }

            return Ok(team);
        }
    }
}
