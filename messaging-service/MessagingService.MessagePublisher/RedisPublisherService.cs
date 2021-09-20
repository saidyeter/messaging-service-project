using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;

namespace MessagingService.MessagePublisher
{

    public class RedisMessagePublisher : IMessagePublisher
    {
        private readonly ConnectionMultiplexer connection;
        public RedisMessagePublisher(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Redis");
            connection = ConnectionMultiplexer.Connect(connectionString);
        }

        public void PublishMessage(string channel, string messageId, string senderUser, string receiverUser)
        {
            var pubsub = connection.GetSubscriber();
            var messageObj = new
            {
                senderUser = senderUser,
                receiverUser = receiverUser,
                messageId = messageId
            };
            pubsub.PublishAsync(channel, System.Text.Json.JsonSerializer.Serialize(messageObj), CommandFlags.FireAndForget);
        }
    }
}
