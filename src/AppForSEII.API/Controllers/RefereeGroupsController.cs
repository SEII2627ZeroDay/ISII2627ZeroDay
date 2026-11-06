using System.Net;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.RefereeDTOs;
using AppForSEII.API.Models;
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

        // GET: api/RefereeGroups/GetRefereeGroup?id=5
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(RefereeGroupDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetRefereeGroup(int id)
        {
            var group = await _context.RefereeGroup.FirstOrDefaultAsync(rg => rg.Id == id);

            if (group == null)
            {
                _logger.LogError($"Error: Referee group with id {id} does not exist");
                return NotFound();
            }

            var assignments = await _context.RefereeAssignedTo
                .Where(a => a.RefereeGroupId == id)
                .OrderBy(a => a.RefereeId)
                .Select(a => new RefereeAssignedToDTO(a.RefereeGroupId, a.RefereeId, a.AcceptedAssignement, a.Role, a.RoleDescription))
                .ToListAsync();

            var refereeGroupDTO = new RefereeGroupDTO(group.Id, group.Name, group.Description, group.Rules, group.GameId, assignments);

            return Ok(refereeGroupDTO);
        }

        // POST: api/RefereeGroups/CreateRefereeGroup
        // Any validation defined with DataAnnotations in the DTOs is checked before running the method.
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(RefereeGroupDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateRefereeGroup(RefereeGroupDTO refereeGroupForCreate)
        {
            if (refereeGroupForCreate.RefereeAssignedTos.Count == 0)
            {
                return BadRequestWithError("RefereeAssignedTos", "You must select at least one referee");
            }

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == refereeGroupForCreate.GameId);
            if (game == null)
            {
                return BadRequestWithError("GameId", "The game does not exist");
            }

            if (await _context.RefereeGroup.AnyAsync(rg => rg.GameId == game.Id))
            {
                return BadRequestWithError("GameId", "The game already has a referee group");
            }

            // the name of the group must be unique
            if (await _context.RefereeGroup.AnyAsync(rg => rg.Name == refereeGroupForCreate.Name))
            {
                return BadRequestWithError("Name", "A referee group with that name already exists");
            }

            var refereeIds = refereeGroupForCreate.RefereeAssignedTos.Select(a => a.RefereeId).ToList();
            if (refereeIds.Distinct().Count() != refereeIds.Count)
            {
                return BadRequestWithError("RefereeAssignedTos", "A referee cannot be selected more than once");
            }

            // the number of referees must be enough for the sport of the game
            var sport = await _context.Sports.FirstAsync(s => s.Id == game.SportId);
            if (refereeIds.Count < sport.NumberofReferees)
            {
                return BadRequestWithError("RefereeAssignedTos", $"The sport requires at least {sport.NumberofReferees} referees");
            }

            // every referee must exist and be instructed in the sport of the game
            int validReferees = await _context.Referee
                .CountAsync(r => refereeIds.Contains(r.Id) && r.SportId == game.SportId && r.YearsRefereeing > 0);
            if (validReferees != refereeIds.Count)
            {
                return BadRequestWithError("RefereeAssignedTos", "Some referees do not exist or are not instructed in the sport of the game");
            }

            // group and assignments are saved together: if something fails, nothing is stored
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var group = new RefereeGroup(refereeGroupForCreate.Name, refereeGroupForCreate.Description,
                refereeGroupForCreate.Rules, game.Id);
            _context.RefereeGroup.Add(group);
            await _context.SaveChangesAsync(); // generates group.Id

            foreach (var assignment in refereeGroupForCreate.RefereeAssignedTos)
            {
                // AcceptedAssignement = false: post-condition, every referee is pending of assignment
                _context.RefereeAssignedTo.Add(new RefereeAssignedTo(group.Id, assignment.RefereeId, false,
                    assignment.Role, assignment.RoleDescription));
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var assignmentsDTO = refereeGroupForCreate.RefereeAssignedTos
                .Select(a => new RefereeAssignedToDTO(group.Id, a.RefereeId, false, a.Role, a.RoleDescription))
                .ToList();
            var refereeGroupDetail = new RefereeGroupDTO(group.Id, group.Name, group.Description, group.Rules, group.GameId, assignmentsDTO);

            return CreatedAtAction("GetRefereeGroup", new { id = group.Id }, refereeGroupDetail);
        }

        private ActionResult BadRequestWithError(string key, string message)
        {
            _logger.LogError($"{DateTime.Now} Error: {message}");
            ModelState.AddModelError(key, message);
            return BadRequest(new ValidationProblemDetails(ModelState));
        }
    }
}