using MessagingService.DataAccess.Model;
using MongoDB.Driver;

namespace MessagingService.DataAccess.Collection
{
    public class AccountRepository : GenericRepository<AccountModel>, IAccountRepository
    {
        private const string DB_COLLECTION = "account";

        public AccountRepository(IMongoClient mongoClient, string dbName= "messagingdb")
            : base(mongoClient, dbName, DB_COLLECTION)
        {
        }

    }

}
