using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.DTOs.ApplicationUserDTO;
using AppForSEII.API.Data;

namespace AppForSEII.API.Controllers
{
    public class ApplicationUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationUsersController> _logger;

        public ApplicationUsersController(ApplicationDbContext context, ILogger<ApplicationUsersController> logger)
        {
            _context = context;
            _logger=logger;
        }

                // GET: api/ApplicationUsers/GetUsersForTeam?sport=Football&skill=4
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<ApplicationUserForTeamDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetUsersForTeam(string? sport, int? skill)
        {
            var users = await _context.ApplicationUsers
                .Include(u => u.Interested)
                    .ThenInclude(i => i.Sport)
                .Where(u =>
                    (sport == null || u.Interested.Any(i => i.Sport.Name == sport))
                    && (skill == null || u.Interested.Any(i => i.Skill == skill)))
                .OrderBy(u => u.Surname)
                    .ThenBy(u => u.Name)
                .Select(u => new ApplicationUserForTeamDTO(
                    u.Id,
                    u.Name ?? "",
                    u.Surname ?? "",
                    u.Age,
                    u.Interested.Where(i => sport == null || i.Sport.Name == sport)
                                .Select(i => i.Sport.Name)
                                .FirstOrDefault() ?? "",
                    u.Interested.Where(i => sport == null || i.Sport.Name == sport)
                                .Select(i => i.Skill)
                                .FirstOrDefault()
                ))
                .ToListAsync();

            return Ok(users);
        }


        // GET: ApplicationUsers
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<ApplicationUserDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Index()
        {
            return Ok(await _context.ApplicationUsers
            .Select(u=>new ApplicationUserDTO(u.Id, u.Name, u.Surname, u.UserName, u.PhoneNumber))
            .ToListAsync());
        }

        // GET: ApplicationUsers/Details/5
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ApplicationUserDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUserDTO = await _context.ApplicationUsers
                .Include(u => u.CaptainOf)
                .Include(u => u.TeamInvitations)
                    .ThenInclude(ti => ti.Team)
                .Where(u => u.Id == id)
                .Select(u=>new ApplicationUserDTO(u.Id, u.Name, u.Surname, u.UserName, u.PhoneNumber))
                .FirstOrDefaultAsync();

            if (applicationUserDTO == null)
            {
                _logger.LogError($"Error: ApplicationUser with id {id} does not exist");

                return NotFound();
            }

            return Ok(applicationUserDTO);
        }


        private bool ApplicationUserDTOExists(string name, string surname)
        {
            return _context.ApplicationUsers.Any(u => u.Name == name && u.Surname==surname);
        }
    }
}
