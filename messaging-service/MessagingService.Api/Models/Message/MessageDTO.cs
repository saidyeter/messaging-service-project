using System;

namespace MessagingService.Api.Models.Message
{
    public class MessageDTO
    {
        public string Id { get; set; }
        public string SenderUser { get; set; }
        public string ReceiverUser { get; set; }
        public string Message { get; set; }
        public DateTime SendedAt { get; set; }
        public DateTime SeenAt {  get; set;}
    }
}
