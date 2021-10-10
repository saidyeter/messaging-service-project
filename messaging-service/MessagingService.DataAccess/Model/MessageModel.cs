using MessagingService.DataAccess.Model.Base;
using System;

namespace MessagingService.DataAccess.Model
{
    public class MessageModel : BaseDocumentModel
    {
        public string SenderUser { get; set; }
        public string ReceiverUser { get; set; }
        public string Message { get; set; }
        public DateTime SendedAt { get; set; }
        public DateTime? SeenAt { get; set; }
    }
}
