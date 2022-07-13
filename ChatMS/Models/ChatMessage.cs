namespace Alfa.ChatMS.Models
{
    public class ChatMessage
    {
        public Guid MessageId { get; set; }
        public string ReceiverConnectionId { get; set; }
        public string Receiver { get; set; }
        public string Message { get; set; }
        public string SenderConnectionId { get; set; }
        public string Sender { get; set; }
    }
}