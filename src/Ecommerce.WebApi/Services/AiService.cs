using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.WebApi.Services
{
    /// <summary>A single chat message exchanged with the AI model.</summary>
    public class ChatMessage
    {
        public string Role { get; set; }      // "system" | "user" | "assistant"
        public string Content { get; set; }
    }

    /// <summary>
    /// Minimal OpenAI-compatible client for the OpenRouter gateway
    /// (https://openrouter.ai). Lets the store call hosted LLM models without
    /// managing API keys per-vendor. Model and key are read from config.
    /// </summary>
    public class AiService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public AiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        /// <summary>
        /// Sends a conversation to the model and returns the assistant's reply.
        /// Throws on missing key or a non-success response from the gateway.
        /// </summary>
        public async Task<string> ChatAsync(IList<ChatMessage> messages, CancellationToken ct = default)
        {
            var key = _config["OpenRouter:ApiKey"];
            if (string.IsNullOrWhiteSpace(key))
                throw new System.InvalidOperationException(
                    "OpenRouter API key is not configured. Set OpenRouter:ApiKey in appsettings.json or an env var.");

            var model = _config["OpenRouter:Model"] ?? "meta-llama/llama-3.1-8b-instruct:free";

            var baseUrl = _config["OpenRouter:BaseUrl"] ?? "https://openrouter.ai/api/v1";
            var endpoint = new System.Uri(new System.Uri(baseUrl.TrimEnd('/') + "/"), "chat/completions");

            var request = new
            {
                model,
                messages = messages.Select(m => new { role = m.Role, content = m.Content }).ToList(),
                max_tokens = 512
            };

            using var http = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = JsonContent.Create(request)
            };
            http.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", key);
            http.Headers.TryAddWithoutValidation("HTTP-Referer", "http://localhost:5173");
            http.Headers.TryAddWithoutValidation("X-Title", "Legacy Ecommerce Store");

            using var response = await _http.SendAsync(http, ct);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                throw new System.InvalidOperationException(
                    $"OpenRouter returned {(int)response.StatusCode}: {body}");
            }

            var result = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(ct);
            return result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim()
                   ?? "(no reply)";
        }

        private sealed class ChatCompletionResponse
        {
            [JsonPropertyName("choices")]
            public List<Choice> Choices { get; set; }
        }

        private sealed class Choice
        {
            [JsonPropertyName("message")]
            public Message Message { get; set; }
        }

        private sealed class Message
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }
        }
    }
}
