using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.Data;
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

        //Slide 11 example: ComputeDivision
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

        //GET GetGame
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

        //POST CreateGame
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(GameDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateGame([FromBody] GameForCreateDTO gameForCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Validamos unicidad del nombre
            var nameExists = await _context.Games.AnyAsync(g => g.Name == gameForCreateDto.Name);
            if (nameExists)
            {
                _logger.LogError($"A game with name '{gameForCreateDto.Name}' already exists.");
                return BadRequest("A game with this name already exists.");
            }

            //Buscamos Deporte y Usuario Responsable
            var sport = await _context.Sports.FirstOrDefaultAsync(s => s.Name == gameForCreateDto.SportName);
            if (sport == null)
            {
                _logger.LogError($"Sport '{gameForCreateDto.SportName}' not found.");
                return BadRequest("The specified sport does not exist.");
            }

            var responsible = await _context.Users.FirstOrDefaultAsync(u => u.UserName == gameForCreateDto.ResponsibleUserName);
            if (responsible == null)
            {
                _logger.LogError($"User '{gameForCreateDto.ResponsibleUserName}' not found.");
                return BadRequest("The responsible user does not exist.");
            }

            //Creamos la entidad Game base
            var newGame = new Game(
                gameForCreateDto.Name,
                gameForCreateDto.Date,
                gameForCreateDto.Place,
                gameForCreateDto.Description,
                sport,
                responsible
            );

            //Procesamos las invitaciones a los equipos
            foreach (var invitationDto in gameForCreateDto.GameInvitations)
            {
                var team = await _context.Teams.FirstOrDefaultAsync(t => t.Name == invitationDto.TeamName);
                if (team == null)
                {
                    _logger.LogError($"Team '{invitationDto.TeamName}' not found. Aborting creation.");
                    return BadRequest($"The team '{invitationDto.TeamName}' does not exist.");
                }

                var invitation = new GameInvitation(team, newGame, invitationDto.InvitationAccepted, invitationDto.InvitationMessage ?? "We invite you to participate!");
                newGame.GameInvitations.Add(invitation);
            }

            //Guardamos en la Base de Datos
            _context.Games.Add(newGame);
            await _context.SaveChangesAsync();

            //Proyectamos el resultado final a un GameDetailDTO
            var resultDto = new GameDetailDTO(
                newGame.Id,
                newGame.Name,
                newGame.Date,
                newGame.Place,
                newGame.Description,
                newGame.Sport.Name,
                newGame.ResponsibleFor.UserName ?? "",
                newGame.GameInvitations.Select(gi => new GameInvitationDTO(
                    gi.Team.Name,
                    gi.AcceptedGame,
                    gi.Message
                )).ToList()
            );

            return CreatedAtAction(nameof(GetGame), new { id = newGame.Id }, resultDto);
        }
    }
}
