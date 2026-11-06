using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.TeamDTOs;
using AppForSEII.API.DTOs.GameDTOs;
using AppForSEII.API.Models;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GamesController> _logger;

        public GamesController(ApplicationDbContext context, ILogger<GamesController> logger)
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

        // POST: api/Teams/CreateTeam
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(TeamDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> CreateTeam([FromBody] TeamForCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for CreateTeam.");
                return BadRequest(ModelState);
            }

            if (dto.MinAge > dto.MaxAge)
            {
                _logger.LogError("MinAge cannot be greater than MaxAge.");
                return BadRequest("Minimum age cannot be greater than maximum age.");
            }

            // Alternative flow 5: Team's name is already chosen
            var nameExists = await _context.Teams.AnyAsync(t => t.Name == dto.Name);
            if (nameExists)
            {
                _logger.LogError($"Team name '{dto.Name}' already exists.");
                return Conflict($"Team name '{dto.Name}' already exists.");
            }

            // Sport existence
            var sport = await _context.Sports.FirstOrDefaultAsync(s => s.Name == dto.Sport || s.Id.ToString() == dto.Sport);
            if (sport == null)
            {
                _logger.LogError($"Sport '{dto.Sport}' not found.");
                return NotFound($"Sport '{dto.Sport}' was not found.");
            }

            // Captain existence
            var captain = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == dto.CaptainId || u.UserName == dto.CaptainId || u.Email == dto.CaptainId);
            if (captain == null)
            {
                _logger.LogError($"Captain '{dto.CaptainId}' not found.");
                return NotFound($"Captain '{dto.CaptainId}' was not found.");
            }

            // Captain age check
            if (captain.Age < dto.MinAge || captain.Age > dto.MaxAge)
            {
                _logger.LogError($"Captain age ({captain.Age}) is outside the team age range ({dto.MinAge}-{dto.MaxAge}).");
                return BadRequest($"Captain's age ({captain.Age}) is outside the team age range ({dto.MinAge}-{dto.MaxAge}).");
            }
            // Calculates the total number of members: 1 for the current player plus the number of invited players (0 if the list is null).
            int totalMembers = 1 + (dto.InvitedPlayers?.Count ?? 0);

            // Alternative flow 4: Number of players invited is too small for the sport selected
            if (totalMembers < sport.MinimumNumberOfPlayers)
            {
                _logger.LogError($"Total team members ({totalMembers}) is less than minimum required ({sport.MinimumNumberOfPlayers}) for {sport.Name}.");
                return BadRequest($"The number of players ({totalMembers}) is too small for {sport.Name}, which requires at least {sport.MinimumNumberOfPlayers} players.");
            }

            // Max members constraint
            if (totalMembers > dto.MaxMembers)
            {
                _logger.LogError($"Total members ({totalMembers}) exceeds MaxMembers ({dto.MaxMembers}).");
                return BadRequest($"Total team members ({totalMembers}) exceeds maximum allowed members ({dto.MaxMembers}).");
            }

            // Check invitations
            var invitedUsers = new List<ApplicationUser>();
            if (dto.InvitedPlayers != null && dto.InvitedPlayers.Count > 0)
            {
                var invitedUserIds = dto.InvitedPlayers.Select(p => p.UserId).ToList();

                // Check for duplicates in invitations
                if (invitedUserIds.Distinct().Count() != invitedUserIds.Count)
                {
                    _logger.LogError("Duplicate users found in invited players list.");
                    return BadRequest("Duplicate invited players in the team.");
                }

                // Captain cannot invite themselves
                if (invitedUserIds.Any(id => id == captain.Id || id == captain.UserName || id == captain.Email))
                {
                    _logger.LogError("Captain cannot be invited as a teammate.");
                    return BadRequest("Captain cannot be invited as a teammate.");
                }

                invitedUsers = await _context.ApplicationUsers
                    .Where(u => invitedUserIds.Contains(u.Id) || invitedUserIds.Contains(u.UserName!) || invitedUserIds.Contains(u.Email!))
                    .ToListAsync();

                if (invitedUsers.Count != invitedUserIds.Count)
                {
                    _logger.LogError("One or more invited users not found in database.");
                    return NotFound("One or more invited users do not exist.");
                }

                foreach (var user in invitedUsers)
                {
                    if (user.Age < dto.MinAge || user.Age > dto.MaxAge)
                    {
                        _logger.LogError($"Invited player '{user.Name} {user.Surname}' age ({user.Age}) does not meet the age criteria ({dto.MinAge}-{dto.MaxAge}).");
                        return BadRequest($"Invited player '{user.Name} {user.Surname}' (age {user.Age}) does not meet the age criteria ({dto.MinAge}-{dto.MaxAge}).");
                    }
                }
            }

            // Create team entity
            var team = new Team(
                dto.Name,
                dto.Description,
                dto.MaxMembers,
                dto.MinAge,
                dto.MaxAge,
                captain,
                sport
            );

            _context.Teams.Add(team);

            // Create team invitations (marked as pending: InvitationAccepted = false)
            if (dto.InvitedPlayers != null)
            {
                foreach (var invitationDto in dto.InvitedPlayers)
                {
                    var user = invitedUsers.First(u => u.Id == invitationDto.UserId || u.UserName == invitationDto.UserId || u.Email == invitationDto.UserId);
                    var invitation = new TeamInvitation(
                        user,
                        team,
                        invitationDto.InvitationMessage,
                        invitationAccepted: false
                    );
                    _context.TeamInvitations.Add(invitation);
                }
            }

            await _context.SaveChangesAsync();

            // Build TeamDetailDTO for response
            var invitedPlayerDTOs = (dto.InvitedPlayers ?? new List<TeamInvitationDTO>()).Select(i =>
            {
                var u = invitedUsers.First(user => user.Id == i.UserId || user.UserName == i.UserId || user.Email == i.UserId);
                return new TeamPlayerDTO(
                    u.Name ?? "",
                    u.Surname ?? "",
                    u.Age,
                    i.InvitationMessage,
                    false
                );
            }).ToList();

            var result = new TeamDetailDTO(
                team.Id,
                team.Name,
                team.Description,
                team.MaxMembers,
                team.MinAge,
                team.MaxAge,
                sport.Name,
                captain.Name ?? "",
                captain.Surname ?? "",
                captain.Age,
                invitedPlayerDTOs
            );

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, result);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(GameDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetGame(int id)
        {
            var gameDto = await _context.Games
                .Include(g => g.Sport)
                .Include(g => g.ResponsibleFor)
                .Include(g => g.GameInvitations)
                    .ThenInclude(gi => gi.Team) 
                .Where(g => g.Id == id)
                .Select(g => new GameDetailDTO(
                    g.Id,
                    g.Name,
                    g.Date,
                    g.Place,
                    g.Description,
                    g.Sport.Name,
                    g.ResponsibleFor.UserName ?? "",
                    g.GameInvitations.Select(gi => new GameInvitationDTO(
                        gi.Team.Name,
                        gi.AcceptedGame,
                        gi.Message
                    )).ToList()
                ))
                .FirstOrDefaultAsync();

            if (gameDto == null)
            {
                _logger.LogError($"Game with id {id} was not found");
                return NotFound();
            }

            return Ok(gameDto);
        }
    }
}
