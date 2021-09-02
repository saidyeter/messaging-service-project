using MessagingService.MongoDB.Model;
using MongoDB.Driver;

namespace MessagingService.MongoDB.Collection
{
    public class UserCollection : BaseDocumentCollection<UserModel>, IUserCollection
    {
        private const string DB_COLLECTION = "User";

        public UserCollection(IMongoClient mongoClient, string dbName= "messagingdb")
            : base(mongoClient, dbName, DB_COLLECTION)
        {
        }

    }

}
