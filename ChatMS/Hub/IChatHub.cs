using Alfa.ChatMS.Models;

namespace Alfa.ChatMS
{
    public interface IChatHub
    {
        Task OnConnectedAsync();
        Task OnDisconnectedAsync(Exception? exception);
        Task SendMessageAsync(ChatMessage message);
    }
}