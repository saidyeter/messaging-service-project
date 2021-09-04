using MessagingService.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace MessagingService.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private IJwtService jwtService;

        public JwtAuthorizeAttribute()
        {


        }
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

            }
            catch (Exception ex)
            {
                //logla
                context.Result = new UnauthorizedResult();

            }

        }
    }
}
