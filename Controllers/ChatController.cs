using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Services;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IOpenAiService _openAiService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(AppDbContext context,IOpenAiService  openAiService, ILogger<ChatController> logger)
        {
            _context = context;
            _logger = logger;
             _openAiService = openAiService;
        }

        // GET: api/appuser
        [HttpGet]
        public async Task<IActionResult> Chat(string UserMessage)
        {
            _logger.LogInformation("Start Chai with Ai Service ");
            
            string aiMessage = await  _openAiService.GetAssistantResponseAsync(UserMessage);
           
            return Ok(aiMessage);
        }

    }
}
