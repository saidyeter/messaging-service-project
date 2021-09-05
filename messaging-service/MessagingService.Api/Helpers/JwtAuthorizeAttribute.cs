using MessagingService.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace MessagingService.Api.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private IJwtService jwtService;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (jwtService is null)
            {
                jwtService = (IJwtService)context.HttpContext.RequestServices.GetService(typeof(IJwtService));
            }

            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token is null)
            {
                context.Result = new UnauthorizedResult();
            }

            try
            {
                var claims = jwtService.Resolve(token);
                if (claims.Count == 0)
                {
                    context.Result = new UnauthorizedResult();
                }
                context.HttpContext.Request.Headers.Add("Id", claims["Id"]);
                context.HttpContext.Request.Headers.Add("UserName", claims["UserName"]);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JWT resolve error: " + ex.Message);
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
