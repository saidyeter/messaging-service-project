using MessagingService.Api.Filters;
using MessagingService.Api.Models.Message;
using MessagingService.Api.Services;
using MessagingService.DataAccess.Collection;
using MessagingService.DataAccess.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingService.Api.Controllers
{
    [JwtAuthorizeAttribute]
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository messageCollection;

        public MessagesController(IMessageRepository messageCollection)
        {
            this.messageCollection = messageCollection;
        }

        /// <summary>
        /// selam
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SendResult Send(SendRequest request)
        {
            var createResult = messageCollection.Create(new MessageModel
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