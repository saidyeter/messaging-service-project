using MessagingService.DataAccess.Model;
using MongoDB.Driver;

namespace MessagingService.DataAccess.Collection
{
    public class MessageRepository : GenericRepository<MessageModel>, IMessageRepository
    {
        private const string DB_COLLECTION = "message";

        public MessageRepository(IMongoClient mongoClient, string dbName = "messagingdb")
            : base(mongoClient, dbName, DB_COLLECTION)
        {
        }

    }

}
