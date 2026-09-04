using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.WebApi.Services;

namespace Ecommerce.WebApi.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private const string SystemPrompt =
            "You are the friendly 'Legacy Store' shopping assistant. You help customers " +
            "with product questions, order status, shipping, coupons, and general store help. " +
            "Be concise, helpful, and on-topic. If asked to do something unsafe or unrelated, " +
            "politely steer the conversation back to shopping. The store has a SAVE10 coupon for " +
            "10% off and free shipping on orders over $75.";

        private readonly AiService _ai;

        public AiController(AiService ai)
        {
            _ai = ai;
        }

        /// <summary>Sends a conversation to the AI assistant and returns the reply.</summary>
        [HttpPost("chat")]
        public async Task<IActionResult> Chat(ChatRequest request, CancellationToken ct = default)
        {
            var messages = new List<ChatMessage> { new ChatMessage { Role = "system", Content = SystemPrompt } };

            if (request?.Messages != null)
                messages.AddRange(request.Messages
                    .Where(m => !string.IsNullOrWhiteSpace(m?.Content))
                    .Select(m => new ChatMessage { Role = m.Role, Content = m.Content }));

            if (messages.Count == 1)
                return BadRequest(new { success = false, message = "No message provided." });

            try
            {
                var reply = await _ai.ChatAsync(messages, ct);
                return Ok(new { success = true, role = "assistant", content = reply });
            }
            catch (System.InvalidOperationException ex)
            {
                return StatusCode(502, new { success = false, message = ex.Message });
            }
        }

        public class ChatRequest
        {
            public List<ChatMessageDto> Messages { get; set; }
        }

        public class ChatMessageDto
        {
            public string Role { get; set; }
            public string Content { get; set; }
        }
    }
}
