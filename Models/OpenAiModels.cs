using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace api.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class RunDetails
    {
        public string Id { get; set; }
        public string AssistantId { get; set; }
        public string ThreadId { get; set; }
        public string Status { get; set; }
        public string Model { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class RunResponse
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string AssistantId { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MessageRequest
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ThreadResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long CreatedAt { get; set; }
        public object Metadata { get; set; }
        public object ToolResources { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ThreadMessage
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long CreatedAt { get; set; }
        public string AssistantId { get; set; }
        public string ThreadId { get; set; }
        public string RunId { get; set; }
        public string Role { get; set; }
        public MessageContent[] Content { get; set; }
        public object[] Attachments { get; set; }
        public object Metadata { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MessagesResponse
    {
        public string Object { get; set; }
        public ThreadMessage[] Data { get; set; }
        public string FirstId { get; set; }
        public string LastId { get; set; }
        public bool HasMore { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MessageContent
    {
        public string Type { get; set; }
        public TextContent Text { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TextContent
    {
        public string Value { get; set; }
        public Annotation[] Annotations { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Annotation
    {
        public string Type { get; set; }
        public string Text { get; set; }
        // Add any other properties specific to annotations
    }

     [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class RunRequest
    {
        public string AssistantId { get; set; }
    }
}
