using System.Threading.Tasks;

namespace api.Services
{
    public interface IOpenAiService
    {
        Task<string> GetAssistantResponseAsync(string userMessage);
    }
}
