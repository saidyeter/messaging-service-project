using MessagingService.DataAccess.Model;
using MongoDB.Driver;
using System.Linq;

namespace MessagingService.DataAccess.Repositories
{
    public class AccountRepository : GenericRepository<AccountModel>, IAccountRepository
    {
        private const string DB_COLLECTION = "account";

        public AccountRepository(IMongoClient mongoClient, string dbName = "messagingdb")
            : base(mongoClient, dbName, DB_COLLECTION)
        {
        }

        public void BlockUser(string userId, string opponent)
        {
            var user = GetById(userId);
            var blockedUser = GetFirstOrDefault(x => x.UserName == opponent);
            if (blockedUser is null)
            {
                throw new System.Exception($"Account doesn't exist ({opponent})");
            }

            user.BlockedUsers.Add(opponent);

            Update(user);
        }

        public bool IsBlocked(string userId, string opponent)
        {
            var user = GetById(userId);
            var blockedUser = GetFirstOrDefault(x => x.UserName == opponent);
            if (blockedUser is null)
            {
                throw new System.Exception($"Account doesn't exist ({opponent})");
            }
            return user.BlockedUsers.Any(x => x == opponent);
        }

        public void UpdateLastLogin(string userId)
        {
            var user = GetById(userId);

            user.LastLogin = System.DateTime.UtcNow;

            Update(user);
        }
    }

}
