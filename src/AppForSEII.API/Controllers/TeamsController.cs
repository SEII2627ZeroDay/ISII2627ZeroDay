using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.DTOs.ApplicationUserDTO;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.TeamDTOs;
using Microsoft.AspNetCore.Components.Web;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TeamsController : ControllerBase
    {
        //used to enable your controller to access to the database
        private readonly ApplicationDbContext _context;
        //used to log any information when your system is running
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(ApplicationDbContext context, ILogger<TeamsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]

        public async Task<ActionResult> ComputeDivision(decimal op1, decimal op2)
        {
            if(op2 == 0)
            {
                _logger.LogError($"{DateTime.Now}Exception: op2=0, division by 0");
                return BadRequest("op2 must be different from 0");
            }
            decimal result = decimal.Round(op1 / op2, 2);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<Team>), (int)HttpStatusCode.OK)]

        public async Task<ActionResult> GetTeamsForGame(string? name, string? skill) //VER DIAPOSITIVAS NUEVAS DEL CAMPUS PARA VER TU EQUIVALENTE EN GAME CREATETEAM
        {
            IList<TeamForGameDTO> teams = await _context.Teams
                                        .Include(t=>t.Sport)
                                        .Where(t=>name.Equals(name) || (name==null))
                                        .OrderBy(t=>t.Name)
                                        .Select(t=>new TeamForGameDTO(t.Id, t.Name, t.MaxMembers, t.MinAge, t.MaxAge, t.Sport.Name))
                                        .ToListAsync();
            return Ok(teams);
        }

    }
}