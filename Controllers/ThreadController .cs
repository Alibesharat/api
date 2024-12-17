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




        [HttpGet("InitThreads")]
        public async Task<IActionResult> InitThreads()
        {
            List<Thread> Threads = new List<Thread>
            {
                new Thread { Id = Guid.NewGuid(), Name = "گمگشتگی", StartUpMessage = "من احساس میکنم گم شدم " },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات رشد عصبی", StartUpMessage = "من اختلالات رشد عصبی دارم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات هوشی", StartUpMessage = "من اختلالات هوشی دارم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات ارتباطی", StartUpMessage = "من اختلالات ارتباطی دارم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال طیف اوتیسم", StartUpMessage = "من دچار اختلال طیف اوتیسم هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال نقص توجه/بیش‌فعالی", StartUpMessage = "من دچار اختلال نقص توجه/بیش‌فعالی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال یادگیری خاص", StartUpMessage = "من اختلال یادگیری خاص دارم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات حرکتی", StartUpMessage = "من دچار اختلالات حرکتی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات طیف اسکیزوفرنی و سایر اختلالات روان‌پریشی", StartUpMessage = "من دچار اختلالات طیف اسکیزوفرنی و سایر اختلالات روان‌پریشی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال شخصیت شیزوتیپی", StartUpMessage = "من دچار اختلال شخصیت شیزوتیپی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال هذیان‌آمیز", StartUpMessage = "من دچار اختلال هذیان‌آمیز هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال روان‌پریشی کوتاه‌مدت", StartUpMessage = "من دچار اختلال روان‌پریشی کوتاه‌مدت هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال اسکیزوفرنیفرم", StartUpMessage = "من دچار اختلال اسکیزوفرنیفرم هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اسکیزوفرنی", StartUpMessage = "من دچار اسکیزوفرنی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال اسکیزوآفیکتیو", StartUpMessage = "من دچار اختلال اسکیزوآفیکتیو هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "کتاتونیا", StartUpMessage = "من دچار کتاتونیا هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات دو قطبی و مرتبط", StartUpMessage = "من دچار اختلالات دو قطبی و مرتبط هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال دو قطبی I", StartUpMessage = "من دچار اختلال دو قطبی I هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال دو قطبی II", StartUpMessage = "من دچار اختلال دو قطبی II هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال سیکلوتمی", StartUpMessage = "من دچار اختلال سیکلوتمی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات افسردگی", StartUpMessage = "من دچار اختلالات افسردگی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال تنظیم خلق و خشم", StartUpMessage = "من دچار اختلال تنظیم خلق و خشم هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال افسردگی عمده", StartUpMessage = "من دچار اختلال افسردگی عمده هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال افسردگی پایدار (دیس‌تیومیا)", StartUpMessage = "من دچار اختلال افسردگی پایدار (دیس‌تیومیا) هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال دیسفوریک پیش از قاعدگی", StartUpMessage = "من دچار اختلال دیسفوریک پیش از قاعدگی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات اضطرابی", StartUpMessage = "من دچار اختلالات اضطرابی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال اضطراب جدایی", StartUpMessage = "من دچار اختلال اضطراب جدایی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال سکوت انتخابی", StartUpMessage = "من دچار اختلال سکوت انتخابی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال فوبیای خاص", StartUpMessage = "من دچار اختلال فوبیای خاص هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال اضطراب اجتماعی", StartUpMessage = "من دچار اختلال اضطراب اجتماعی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال هراس", StartUpMessage = "من دچار اختلال هراس هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال آگورافوبیا", StartUpMessage = "من دچار اختلال آگورافوبیا هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال اضطراب فراگیر", StartUpMessage = "من دچار اختلال اضطراب فراگیر هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال وسواسی-جبری و مرتبط", StartUpMessage = "من دچار اختلال وسواسی-جبری و مرتبط هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال وسواسی-جبری", StartUpMessage = "من دچار اختلال وسواسی-جبری هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال بدنی دیسمورفیک", StartUpMessage = "من دچار اختلال بدنی دیسمورفیک هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال انباشت", StartUpMessage = "من دچار اختلال انباشت هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "تریکوتیلومانیا", StartUpMessage = "من دچار تریکوتیلومانیا هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال اکسکوریاسیون", StartUpMessage = "من دچار اختلال اکسکوریاسیون هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات تروماتیک و استرس‌زا", StartUpMessage = "من دچار اختلالات تروماتیک و استرس‌زا هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال چسبندگی واکنشی", StartUpMessage = "من دچار اختلال چسبندگی واکنشی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال مشارکت اجتماعی غیرکنترل شده", StartUpMessage = "من دچار اختلال مشارکت اجتماعی غیرکنترل شده هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال PTSD", StartUpMessage = "من دچار PTSD هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال استرس حاد", StartUpMessage = "من دچار اختلال استرس حاد هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال تنظیم", StartUpMessage = "من دچار اختلال تنظیم هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات جداکننده", StartUpMessage = "من دچار اختلالات جداکننده هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال هویت جداکننده", StartUpMessage = "من دچار اختلال هویت جداکننده هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال فراموشی جداکننده", StartUpMessage = "من دچار اختلال فراموشی جداکننده هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال افسردگی/واقعه‌ای", StartUpMessage = "من دچار اختلال افسردگی/واقعه‌ای هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات سماتیک و جسمی مرتبط", StartUpMessage = "من دچار اختلالات سماتیک و جسمی مرتبط هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال سماتیک-سمی", StartUpMessage = "من دچار اختلال سماتیک-سمی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال بدنی-سمی", StartUpMessage = "من دچار اختلال بدنی-سمی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال تبدیل", StartUpMessage = "من دچار اختلال تبدیل هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال فیکتیسیوز", StartUpMessage = "من دچار اختلال فیکتیسیوز هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال اضطراب جدایی", StartUpMessage = "من دچار اختلال اضطراب جدایی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال چسبندگی واکنشی", StartUpMessage = "من دچار اختلال چسبندگی واکنشی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال مشارکت اجتماعی غیرکنترل شده", StartUpMessage = "من دچار اختلال مشارکت اجتماعی غیرکنترل شده هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال اجتناب/محدودیت مصرف غذا", StartUpMessage = "من دچار اختلال اجتناب/محدودیت مصرف غذا هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "دیسفوریا جنسیتی", StartUpMessage = "من دچار دیسفوریا جنسیتی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال انباشت", StartUpMessage = "من دچار اختلال انباشت هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات خوردن", StartUpMessage = "من دچار اختلالات خوردن هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال شخصیت مرزی", StartUpMessage = "من دچار اختلال شخصیت مرزی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال شخصیت شیزووتیپی", StartUpMessage = "من دچار اختلال شخصیت شیزووتیپی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال هذیان‌آمیز", StartUpMessage = "من دچار اختلال هذیان‌آمیز هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال اسکیزوفرنیفرم", StartUpMessage = "من دچار اختلال اسکیزوفرنیفرم هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال اسکیزوآفیکتیو", StartUpMessage = "من دچار اختلال اسکیزوآفیکتیو هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال کتاتونیا", StartUpMessage = "من دچار کتاتونیا هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلالات دو قطبی و مرتبط", StartUpMessage = "من دچار اختلالات دو قطبی و مرتبط هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال دو قطبی I", StartUpMessage = "من دچار اختلال دو قطبی I هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال دو قطبی II", StartUpMessage = "من دچار اختلال دو قطبی II هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال سیکلوتمی", StartUpMessage = "من دچار اختلال سیکلوتمی هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال افسردگی عمده", StartUpMessage = "من دچار اختلال افسردگی عمده هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال افسردگی پایدار (دیس‌تیومیا)", StartUpMessage = "من دچار اختلال افسردگی پایدار (دیس‌تیومیا) هستم." },
                new Thread { Id = Guid.NewGuid(), Name = "اختلال دیسفوریک پیش از قاعدگی", StartUpMessage = "من دچار اختلال دیسفوریک پیش از قاعدگی هستم." }
            };

            // Add the Threads to the context
            _context.Threads.AddRange(Threads);

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Threads initialized successfully.", Count = Threads.Count });
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., database errors)
                return StatusCode(500, new { Message = "An error occurred while initializing threads.", Details = ex.Message });
            }

        }
    }
}
