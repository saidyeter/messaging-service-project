namespace MessagingService.Api.Models.Message
{
    public class SendRequest
    {
        public string ReceiverUser { get; set; }
        public string Message { get; set; }
    }
}
