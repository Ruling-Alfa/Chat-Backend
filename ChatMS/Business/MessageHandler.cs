using Alfa.ChatMS.Models;
using Microsoft.AspNetCore.SignalR;

namespace Alfa.ChatMS
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IHubContext<ChatHub> _chatHubContext;
        public MessageHandler(IHubContext<ChatHub> chatHubContext)
        {
            _chatHubContext = chatHubContext;
        }

        public Task SendMessageToAll(ChatMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            else
            {
                return _chatHubContext.Clients.All.SendAsync("ReceiveMsg", message);
            }
        }

        public Task SendMessageToUser(ChatMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            else if (string.IsNullOrEmpty(message.Receiver))
            {
                throw new ArgumentNullException(nameof(message.Receiver));
            }
            else
            {
                return _chatHubContext.Clients.User(message.Receiver).SendAsync("ReceiveMsg", message);
            }
        }

        public Task SendMessageToConnection(ChatMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            else if (string.IsNullOrEmpty(message.Receiver))
            {
                throw new ArgumentNullException(nameof(message.ReceiverConnectionId));
            }
            else
            {
                return _chatHubContext.Clients.Client(message.ReceiverConnectionId).SendAsync("ReceiveMsg", message);
            }
        }

        public Task SendMessage(ChatMessage message)
        {
            Task returnTask;
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            else if (!string.IsNullOrEmpty(message.Receiver))
            {
                returnTask = this.SendMessageToUser(message);
            }
            else if (!string.IsNullOrEmpty(message.ReceiverConnectionId))
            {
                returnTask = this.SendMessageToConnection(message);
            }
            else
            {
                returnTask = this.SendMessageToAll(message);
            }
            return returnTask;
        }
    }
}