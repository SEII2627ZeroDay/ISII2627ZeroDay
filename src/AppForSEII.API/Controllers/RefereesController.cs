
namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefereesController : ControllerBase
    {
        private readonly  ApplicationDbContext _context;
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
    }
}