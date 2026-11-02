using System.Net;
using AppForSEII.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefereesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RefereesController> _logger;

        public RefereesController(ApplicationDbContext context, ILogger<RefereesController> logger)
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
            if (op2 == 0)
            {
                _logger.LogError($"{DateTime.Now} Exception: op2=0, division by 0");
                return BadRequest("op2 must be different from 0");
            }
            decimal result = decimal.Round(op1 / op2, 2);
            return Ok(result);
        }

        // Referees instructed in a sport (years refereeing > 0).
        // Optional filter: minimum years refereeing (higher or equal).
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetRefereesForRefereeGroup(int sportId, int? minYearsRefereeing = null)
        {
            if (!await _context.Sports.AnyAsync(s => s.Id == sportId))
            {
                _logger.LogError($"{DateTime.Now} Exception: sport {sportId} does not exist");
                return BadRequest("The sport does not exist");
            }

            var query = _context.Referee
                .Where(r => r.SportId == sportId && r.YearsRefereeing > 0);

            if (minYearsRefereeing.HasValue)
            {
                query = query.Where(r => r.YearsRefereeing >= minYearsRefereeing.Value);
            }

            var referees = await query
                .Select(r => new { r.Id, r.Name, r.Surname, r.YearsRefereeing, r.Rating })
                .ToListAsync();

            if (referees.Count == 0)
            {
                _logger.LogError($"{DateTime.Now} Exception: no referees available for sport {sportId}");
                return BadRequest("There are no referees available for this sport");
            }

            return Ok(referees);
        }
    }
}