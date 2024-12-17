using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppUserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AppUserController> _logger;

        public AppUserController(AppDbContext context, ILogger<AppUserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/appuser
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Fetching all users...");
            var users = await _context.AppUsers.ToListAsync();
            return Ok(users);
        }

        // GET: api/appuser/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            _logger.LogInformation("Fetching user with ID: {Id}", id);
            var user = await _context.AppUsers.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID: {Id} not found.", id);
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/appuser
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] AppUser user)
        {
            _logger.LogInformation("Creating a new user...");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user.");
                return BadRequest(ModelState);
            }

            user.Id = Guid.NewGuid();
            user.JoinedDate = DateTime.UtcNow;

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User created successfully with ID: {Id}", user.Id);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        // PUT: api/appuser/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] AppUser updatedUser)
        {
            _logger.LogInformation("Updating user with ID: {Id}", id);
            if (id != updatedUser.Id)
            {
                _logger.LogWarning("User ID mismatch. Provided ID: {Id}", id);
                return BadRequest("User ID mismatch");
            }

            _context.Entry(updatedUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("User with ID: {Id} updated successfully.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.AppUsers.Any(e => e.Id == id))
                {
                    _logger.LogError("User with ID: {Id} not found during update.", id);
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/appuser/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            _logger.LogInformation("Deleting user with ID: {Id}", id);
            var user = await _context.AppUsers.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID: {Id} not found for deletion.", id);
                return NotFound();
            }

            _context.AppUsers.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User with ID: {Id} deleted successfully.", id);
            return NoContent();
        }
    }
}
