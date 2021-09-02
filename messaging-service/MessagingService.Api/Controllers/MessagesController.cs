using MessagingService.MongoDB.Collection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController: ControllerBase
    {
        private readonly IMessageCollection messageCollection;

        public MessagesController(IMessageCollection messageCollection)
        {
            this.messageCollection = messageCollection;
        }

        [HttpPost]
        public SendResult Send(SendRequest request)
        {
            var createResult = messageCollection.Create(new MongoDB.Model.MessageModel
            {
                Message = request.Message,
                ReceiverUser = request.ReceiverUser,
                SenderUser = "sdfsdf"
            });

            return new SendResult
            {
                Message = createResult.Message,
                ReceiverUser = createResult.ReceiverUser,
                MessageId = createResult.Id.ToString()
            };
            //gelirken yetkisi var mı attribute ile kontrol et
            //veritabanına yaz
            //event fırlat
            //log tut
        }
    }
    public class ActionFilterExample : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // context.HttpContext.Request.Headers
            // our code before action executes
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // our code after action executes
        }
    }

    public class SendResult
    {
        public string MessageId { get; set; }
        public string ReceiverUser { get; set; }
        public string Message { get; set; }
    }
    public class SendRequest
    {
        public string ReceiverUser { get; set; }
        public string Message { get; set; }
    }
}
/*
mesaj tablosu
id
sender
receiver
message

user tablosu
id
username
email
password hash
 
 */