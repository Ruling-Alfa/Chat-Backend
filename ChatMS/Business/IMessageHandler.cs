using Alfa.ChatMS.Models;

namespace Alfa.ChatMS
{
    public interface IMessageHandler
    {
        Task SendMessage(ChatMessage message);
        Task SendMessageToAll(ChatMessage message);
        Task SendMessageToConnection(ChatMessage message);
        Task SendMessageToUser(ChatMessage message);
    }
}