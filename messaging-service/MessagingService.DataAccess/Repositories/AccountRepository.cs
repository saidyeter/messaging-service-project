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

        public void UpdateLastMessage(string userA, string userB)
        {
            var aUser = GetFirstOrDefault(x => x.UserName == userA);
            if (aUser.ChattedUsers is null)
            {
                aUser.ChattedUsers = new System.Collections.Generic.List<ChattedUser>();
            }

            if (aUser.ChattedUsers.Any(x => x.UserName == userB))
            {
                aUser.ChattedUsers.First(x => x.UserName == userB).LastMessaged = System.DateTime.UtcNow;
            }
            else
            {
                aUser.ChattedUsers.Add(new ChattedUser
                {
                    UserName= userB,
                    LastMessaged = System.DateTime.UtcNow
                });
            }
            Update(aUser);

            var bUser = GetFirstOrDefault(x => x.UserName == userB);
            if (bUser.ChattedUsers is null)
            {
                bUser.ChattedUsers = new System.Collections.Generic.List<ChattedUser>();
            }
            if (bUser.ChattedUsers.Any(x => x.UserName == userA))
            {
                bUser.ChattedUsers.First(x => x.UserName == userA).LastMessaged = System.DateTime.UtcNow;
            }
            else
            {
                bUser.ChattedUsers.Add(new ChattedUser
                {
                    UserName = userA,
                    LastMessaged = System.DateTime.UtcNow
                });
            }
            Update(bUser);
        }
    }

}
