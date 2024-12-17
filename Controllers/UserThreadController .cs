using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserThreadController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserThreadController> _logger;

        public UserThreadController(AppDbContext context, ILogger<UserThreadController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/userthread
        [HttpGet]
        public async Task<IActionResult> GetAllUserThreads()
        {
            _logger.LogInformation("Fetching all user-thread associations...");
            var userThreads = await _context.Set<UserThread>().ToListAsync();
            return Ok(userThreads);
        }

        // GET: api/userthread/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserThreadById(string id)
        {
            _logger.LogInformation("Fetching user-thread association with ID: {Id}", id);
            var userThread = await _context.Set<UserThread>().FindAsync(id);
            if (userThread == null)
            {
                _logger.LogWarning("User-thread association with ID: {Id} not found.", id);
                return NotFound();
            }

            return Ok(userThread);
        }

        // POST: api/userthread
        [HttpPost]
        public async Task<IActionResult> CreateUserThread([FromBody] UserThread userThread)
        {
            _logger.LogInformation("Creating a new user-thread association...");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UserThread.");
                return BadRequest(ModelState);
            }

            userThread.Id = Guid.NewGuid().ToString();
            _context.Set<UserThread>().Add(userThread);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User-thread association created successfully with ID: {Id}", userThread.Id);
            return CreatedAtAction(nameof(GetUserThreadById), new { id = userThread.Id }, userThread);
        }

        // DELETE: api/userthread/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserThread(string id)
        {
            _logger.LogInformation("Deleting user-thread association with ID: {Id}", id);
            var userThread = await _context.Set<UserThread>().FindAsync(id);
            if (userThread == null)
            {
                _logger.LogWarning("User-thread association with ID: {Id} not found.", id);
                return NotFound();
            }

            _context.Set<UserThread>().Remove(userThread);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User-thread association with ID: {Id} deleted successfully.", id);
            return NoContent();
        }
    }
}
