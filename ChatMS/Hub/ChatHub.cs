using Alfa.ChatMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace Alfa.ChatMS
{
    [Authorize]
    public class ChatHub : Hub, IChatHub
    {
        public override async Task OnConnectedAsync()
        {
            //await Groups.AddToGroupAsync(Context.ConnectionId, "AllUsers");
            await Clients.All.SendAsync("NewUserConnected",
                    new
                    {
                        userIdentifier = Context.UserIdentifier,
                        userName = Context?.User?.FindFirst("UserName")?.Value,
                        connectionId = Context.ConnectionId,
                        Message = $"{Context?.User?.FindFirst("UserName")?.Value} connected on {Context.ConnectionId}"
                    });
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("UserDisconnected",
                    new
                    {
                        userIdentifier = Context.UserIdentifier,
                        userName = Context?.User?.FindFirst("UserName")?.Value,
                        connectionId = Context.ConnectionId,
                        Message = $"{Context?.User?.FindFirst("UserName")?.Value}  dis-connected on {Context.ConnectionId}"
                    });
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AllUsers");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageAsync(ChatMessage message)
        {
            await Clients.User(message.Receiver).SendAsync("ReceiveMsg", new ChatMessage
            {
                MessageId = message.MessageId,
                Sender = Context.UserIdentifier,
                Receiver = message.Receiver,
                SenderConnectionId = Context.ConnectionId,
                ReceiverConnectionId = message.ReceiverConnectionId,
                Message = message.Message
            });
            await Clients.Caller.SendAsync("MessageSent", message.MessageId);
        }
    }
}