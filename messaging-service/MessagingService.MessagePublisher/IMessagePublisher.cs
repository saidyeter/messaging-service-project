namespace MessagingService.MessagePublisher
{
    public interface IMessagePublisher
    {
        void PublishMessage(string channel, string messageId, string senderUser, string receiverUser);
    }
}
