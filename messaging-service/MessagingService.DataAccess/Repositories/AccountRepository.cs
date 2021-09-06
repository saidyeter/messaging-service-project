using MessagingService.DataAccess.Model;
using MessagingService.DataAccess.Repositories.Base;
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
            if (user.BlockedUsers is null)
            {
                user.BlockedUsers = new System.Collections.Generic.List<string>();
            }
            user.BlockedUsers.Add(opponent);

            Update(user);
        }

        public bool IsBlocked(string userId, string opponent)
        {
            var senderUser = GetById(userId);
            var opponentUser = GetFirstOrDefault(x => x.UserName == opponent);
            if (opponentUser is null)
            {
                throw new System.Exception($"Account doesn't exist ({opponent})");
            }


            if (opponentUser.BlockedUsers is null)
            {
                return false;
            } 

            return  opponentUser.BlockedUsers.Any(x => x == senderUser.UserName);
        }

        public void UpdateLastLogin(string userId)
        {
            var user = GetById(userId);

            user.LastLogin = System.DateTime.UtcNow;

            Update(user);
        }
    }

}
