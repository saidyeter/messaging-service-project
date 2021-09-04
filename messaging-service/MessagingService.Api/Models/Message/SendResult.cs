namespace MessagingService.Api.Models.Message
{
    public class SendResult
    {
        public string MessageId { get; set; }
        public string ReceiverUser { get; set; }
        public string Message { get; set; }
    }
}
