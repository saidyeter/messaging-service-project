namespace MessagingService.DataAccess.Model
{
    public class AccountModel : BaseDocumentModel
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}
