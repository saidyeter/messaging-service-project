namespace MessagingService.MongoDB.Model
{
    public class UserModel : BaseDocumentModel
    {
        public string UserName { get; set; }
        public string EMail { get; set; }
        public string PasswordHash { get; set; }
    }
}
