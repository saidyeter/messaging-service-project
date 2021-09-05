using MessagingService.DataAccess.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MessagingService.DataAccess.Repositories
{
    public class MessageRepository : GenericRepository<MessageModel>, IMessageRepository
    {
        private const string DB_COLLECTION = "message";
        private const int MAX_MESSAGE_COUNT = 15;

        public MessageRepository(IMongoClient mongoClient, string dbName = "messagingdb")
            : base(mongoClient, dbName, DB_COLLECTION)
        {
        }

        public string GetLatestMessage(string user, string opponent)
        {
            string message;
            try
            {
                message = mongoCollection.
                     Find(x => (x.ReceiverUser == user && x.SenderUser == opponent) ||
                               (x.ReceiverUser == opponent && x.SenderUser == user)).
                     SortByDescending(x => x.Id).
                     First().
                     Id.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't find message. user: '{user}', opponent : '{opponent}'", ex);
            }

            return message;
        }

        public List<string> GetOlderMessages(string id, string username)
        {
            var message = GetSingleMessage(id, username);

            List<string> messageList;
            try
            {
                var fields = Builders<MessageModel>.Projection.Include(p => p.Id);

                messageList = mongoCollection.
                      Find(x => x.InsertAt < message.InsertAt && (
                                    (x.ReceiverUser == message.ReceiverUser && x.SenderUser == message.SenderUser) ||
                                    (x.ReceiverUser == message.SenderUser && x.SenderUser == message.ReceiverUser))).
                      Limit(MAX_MESSAGE_COUNT).
                      Project<MessageModel>(fields).
                      ToList().
                      Select(x => x.Id.ToString()).
                      ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't get older messages. id : '{id}', username : '{username}'", ex);
            }

            return messageList;
        }

        public MessageModel GetSingleMessage(string id, string username)
        {
            var message = GetById(id);

            if (message is null || (message.SenderUser != username && message.ReceiverUser != username))
            {
                throw new Exception($"Couldn't find message for user. id : '{id}', username : '{username}'");
            }
            return message;
        }
    }

}
