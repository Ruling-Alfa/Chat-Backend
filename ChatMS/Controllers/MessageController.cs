using Alfa.ChatMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Alfa.ChatMS.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[action]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageHandler _messageHandler;
        public MessageController(IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            try
            {
                message = PopulateProperties(message);
                await _messageHandler.SendMessage(message);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        private ChatMessage PopulateProperties(ChatMessage message)
        {
            message.Sender = User.Identity.Name;
            //message.SenderConnectionId = Context.ConnectionId;

            return message;
        }
    }
}