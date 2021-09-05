namespace MessagingService.Api.Models.Message
{
    public class SendResponse
    {
        public string MessageId { get; set; }
        public string ReceiverUser { get; set; }
        public string Message { get; set; }
    }
}
