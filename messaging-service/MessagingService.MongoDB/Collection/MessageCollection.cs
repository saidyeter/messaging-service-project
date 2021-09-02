using MessagingService.MongoDB.Model;
using MongoDB.Driver;

namespace MessagingService.MongoDB.Collection
{
    public class MessageCollection : BaseDocumentCollection<MessageModel>, IMessageCollection
    {
        private const string DB_COLLECTION = "Message";

        public MessageCollection(IMongoClient mongoClient, string dbName = "messagingdb")
            : base(mongoClient, dbName, DB_COLLECTION)
        {
        }

    }

}
