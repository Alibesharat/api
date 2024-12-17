using System;

namespace api.Configuration
{
    public class OpenAiSettings
    {
        public string ApiKey { get; set; }
        public string AssistantId { get; set; }
        public string BaseUrl { get; set; } = "https://api.openai.com/v1";
        public string AssistantsApiVersion { get; set; } = "assistants=v2";
    }
}
