using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using api.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using api.Models;

namespace api.Services
{
    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAiSettings _settings;
        private readonly ILogger<OpenAiService> _logger;

        private string _threadId = string.Empty;

        public OpenAiService(HttpClient httpClient, IOptions<OpenAiSettings> options, ILogger<OpenAiService> logger)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _logger = logger;

            // Configure HttpClient defaults
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", _settings.AssistantsApiVersion);

            // Configure JSON serialization settings globally if not already done
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        /// <summary>
        /// Get assistant response for a user message
        /// </summary>
        public async Task<string> GetAssistantResponseAsync(string userMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(_threadId))
                {
                    _threadId = await CreateThreadAsync();
                }

                await AddMessageToThreadAsync(userMessage, _threadId);

                var runResponse = await CreateRunAsync(_threadId);

                await WaitForRunCompletionAsync(_threadId, runResponse.Id);

                var messages = await GetMessagesAsync(_threadId);
                return messages.Data.FirstOrDefault()?.Content.FirstOrDefault()?.Text?.Value 
                       ?? "No response from assistant.";
            }
            catch (Exception ex)
            {
                LogException(ex);
                return $"Assistant error: {ex.Message}";
            }
        }

        /// <summary>
        /// Wait for run completion with polling
        /// </summary>
        private async Task WaitForRunCompletionAsync(string threadId, string runId, int pollingIntervalSeconds = 1)
        {
            while (true)
            {
                var status = await GetRunDetailsAsync(threadId, runId);

                if (status?.Status == "completed")
                {
                    _logger.LogInformation("Run is completed!");
                    return;
                }

                _logger.LogInformation("Run is still in progress. Waiting...");
                await Task.Delay(TimeSpan.FromSeconds(pollingIntervalSeconds));
            }
        }

        /// <summary>
        /// Get run details
        /// </summary>
        private async Task<RunDetails> GetRunDetailsAsync(string threadId, string runId)
        {
            try
            {
                string url = $"/threads/{threadId}/runs/{runId}";
                var response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<RunDetails>(responseBody);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Create a new thread
        /// </summary>
        private async Task<string> CreateThreadAsync()
        {
            try
            {
                string url = "/threads";
                var response = await _httpClient.PostAsync(url, new StringContent("{}", Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var threadResponse = JsonConvert.DeserializeObject<ThreadResponse>(responseBody);
                return threadResponse?.Id ?? throw new InvalidOperationException("Failed to create thread");
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Add a message to a thread
        /// </summary>
        public async Task AddMessageToThreadAsync(string userMessage, string threadId)
        {
            try
            {
                string url = $"/threads/{threadId}/messages";

                var messageRequest = new MessageRequest
                {
                    Role = "user",
                    Content = userMessage
                };

                var jsonContent = JsonConvert.SerializeObject(messageRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Create a run for a thread
        /// </summary>
        private async Task<RunResponse> CreateRunAsync(string threadId)
        {
            try
            {
                string url = $"/threads/{threadId}/runs";

                var runRequest = new RunRequest
                {
                    AssistantId = _settings.AssistantId,
                };
                var jsonContent = JsonConvert.SerializeObject(runRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<RunResponse>(responseBody)
                    ?? throw new InvalidOperationException("Failed to create run");
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieve messages from a thread
        /// </summary>
        private async Task<MessagesResponse> GetMessagesAsync(string threadId)
        {
            try
            {
                string url = $"/threads/{threadId}/messages";
                var response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Messages Response: {responseBody}", responseBody);

                return JsonConvert.DeserializeObject<MessagesResponse>(responseBody)
                    ?? throw new InvalidOperationException("Failed to retrieve messages");
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        // Logging methods
        private void LogException(Exception ex)
        {
            _logger.LogError(ex, "An error occurred in OpenAiService");
        }
    }

 
}
