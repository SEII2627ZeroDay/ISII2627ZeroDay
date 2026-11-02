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
                .OrderBy(a => a.RefereeId)
                .Select(a => new RefereeAssignedToDTO(a.RefereeGroupId, a.RefereeId, a.AcceptedAssignement, a.Role, a.RoleDescription))
                .ToListAsync();

            var dto = new RefereeGroupDTO(group.Id, group.Name, group.Description, group.Rules, group.GameId, assignments);

            return Ok(dto);
        }

        // Creates a referee group for a game and assigns the selected referees to it.
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(RefereeGroupDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<RefereeGroupDTO>> CreateRefereeGroup([FromBody] RefereeGroupDTO dto)
        {
            // Mandatory data missing or invalid is already rejected with a 400 by
            // [ApiController] thanks to the DataAnnotations of the DTOs.
            if (dto.RefereeAssignedTos.Count == 0)
            {
                return BadRequest("You must select at least one referee");
            }

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == dto.GameId);
            if (game == null)
            {
                _logger.LogError($"{DateTime.Now} Exception: game {dto.GameId} does not exist");
                return BadRequest("The game does not exist");
            }

            if (await _context.RefereeGroup.AnyAsync(rg => rg.GameId == game.Id))
            {
                return BadRequest("The game already has a referee group");
            }

            // Alternative flow 6: the name of the group must be unique
            if (await _context.RefereeGroup.AnyAsync(rg => rg.Name == dto.Name))
            {
                _logger.LogError($"{DateTime.Now} Exception: referee group name '{dto.Name}' already exists");
                return BadRequest("A referee group with that name already exists");
            }

            var refereeIds = dto.RefereeAssignedTos.Select(a => a.RefereeId).ToList();
            if (refereeIds.Distinct().Count() != refereeIds.Count)
            {
                return BadRequest("A referee cannot be selected more than once");
            }

            // Alternative flow 5: number of referees too small for the sport of the game
            var sport = await _context.Sports.FirstOrDefaultAsync(s => s.Id == game.SportId);
            if (sport == null)
            {
                return BadRequest("The sport of the game does not exist");
            }

            if (refereeIds.Count < sport.NumberofReferees)
            {
                _logger.LogError($"{DateTime.Now} Exception: {refereeIds.Count} referees selected, {sport.NumberofReferees} needed");
                return BadRequest($"The sport requires at least {sport.NumberofReferees} referees");
            }

            // Every referee must exist and be instructed in the sport of the game
            int validReferees = await _context.Referee
                .CountAsync(r => refereeIds.Contains(r.Id) && r.SportId == game.SportId && r.YearsRefereeing > 0);
            if (validReferees != refereeIds.Count)
            {
                return BadRequest("Some referees do not exist or are not instructed in the sport of the game");
            }

            // Group and assignments are saved together: if something fails, nothing is stored.
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var group = new RefereeGroup
            {
                Name = dto.Name,
                Description = dto.Description,
                Rules = dto.Rules,
                GameId = game.Id
            };
            _context.RefereeGroup.Add(group);
            await _context.SaveChangesAsync(); // generates group.Id

            foreach (var a in dto.RefereeAssignedTos)
            {
                _context.RefereeAssignedTo.Add(new RefereeAssignedTo
                {
                    RefereeGroupId = group.Id,
                    RefereeId = a.RefereeId,
                    AcceptedAssignement = false, // post-condition: pending of assignment
                    Role = a.Role,
                    RoleDescription = a.RoleDescription
                });
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var assignments = dto.RefereeAssignedTos
                .Select(a => new RefereeAssignedToDTO(group.Id, a.RefereeId, false, a.Role, a.RoleDescription))
                .ToList();
            var result = new RefereeGroupDTO(group.Id, group.Name, group.Description, group.Rules, group.GameId, assignments);

            return CreatedAtAction(nameof(GetRefereeGroup), new { id = group.Id }, result);
        }
    }
}