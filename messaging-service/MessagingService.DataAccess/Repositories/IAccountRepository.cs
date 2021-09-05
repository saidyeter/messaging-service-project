using MessagingService.DataAccess.Model;

namespace MessagingService.DataAccess.Repositories
{
    public interface IAccountRepository : IGenericRepository<AccountModel>
    {
        void BlockUser(string userId, string opponent);
        void UpdateLastLogin(string userId);
        bool IsBlocked(string userId, string opponent);
    }

}
