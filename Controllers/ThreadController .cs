using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThreadController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ThreadController> _logger;

        public ThreadController(AppDbContext context, ILogger<ThreadController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/thread
        [HttpGet]
        public async Task<IActionResult> GetAllThreads()
        {
            _logger.LogInformation("Fetching all threads...");
            var threads = await _context.Threads.ToListAsync();
            return Ok(threads);
        }

        // GET: api/thread/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetThreadById(Guid id)
        {
            _logger.LogInformation("Fetching thread with ID: {Id}", id);
            var thread = await _context.Threads.FindAsync(id);
            if (thread == null)
            {
                _logger.LogWarning("Thread with ID: {Id} not found.", id);
                return NotFound();
            }
            return Ok(thread);
        }

        // POST: api/thread
        [HttpPost]
        public async Task<IActionResult> CreateThread([FromBody] Thread thread)
        {
            _logger.LogInformation("Creating a new thread...");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for thread.");
                return BadRequest(ModelState);
            }

            thread.Id = Guid.NewGuid();
            _context.Threads.Add(thread);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Thread created successfully with ID: {Id}", thread.Id);
            return CreatedAtAction(nameof(GetThreadById), new { id = thread.Id }, thread);
        }

        // PUT: api/thread/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateThread(Guid id, [FromBody] Thread updatedThread)
        {
            _logger.LogInformation("Updating thread with ID: {Id}", id);
            if (id != updatedThread.Id)
            {
                _logger.LogWarning("Thread ID mismatch. Provided ID: {Id}", id);
                return BadRequest("Thread ID mismatch");
            }

            _context.Entry(updatedThread).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thread with ID: {Id} updated successfully.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Threads.Any(e => e.Id == id))
                {
                    _logger.LogError("Thread with ID: {Id} not found during update.", id);
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/thread/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThread(Guid id)
        {
            _logger.LogInformation("Deleting thread with ID: {Id}", id);
            var thread = await _context.Threads.FindAsync(id);
            if (thread == null)
            {
                _logger.LogWarning("Thread with ID: {Id} not found for deletion.", id);
                return NotFound();
            }

            _context.Threads.Remove(thread);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Thread with ID: {Id} deleted successfully.", id);
            return NoContent();
        }
    }
}
