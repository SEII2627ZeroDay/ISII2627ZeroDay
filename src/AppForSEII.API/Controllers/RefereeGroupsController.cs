using System.Net;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.RefereeDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefereeGroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RefereeGroupsController> _logger;

        public RefereeGroupsController(ApplicationDbContext context, ILogger<RefereeGroupsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Details of a referee group and its assigned referees.
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(RefereeGroupDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<RefereeGroupDTO>> GetRefereeGroup(int id)
        {
            var group = await _context.RefereeGroup.FirstOrDefaultAsync(rg => rg.Id == id);

            if (group == null)
            {
                _logger.LogError($"{DateTime.Now} Exception: referee group {id} not found");
                return NotFound("The referee group does not exist");
            }

            var assignments = await _context.RefereeAssignedTo
                .Where(a => a.RefereeGroupId == id)
                .Select(a => new RefereeAssignedToDTO(a.RefereeGroupId, a.RefereeId, a.AcceptedAssignement, a.Role, a.RoleDescription))
                .ToListAsync();

            var dto = new RefereeGroupDTO(group.Id, group.Name, group.Description, group.Rules, group.GameId, assignments);

            return Ok(dto);
        }
    }
}