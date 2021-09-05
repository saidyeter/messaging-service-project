using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingService.Api.Helpers
{
    public class GlobalActionFilter : IActionFilter
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
