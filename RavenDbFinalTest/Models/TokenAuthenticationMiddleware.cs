using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RavenDbFinalTest.Models
{
    public class TokenAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                return;
            }

            var token = authorizationHeader.ToString().Split(' ')[1];

            // TODO: Validate the token
            // If the token is invalid, return a 401 Unauthorized response

            // Example validation code:
            if (token != "mySecretToken")
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
            else
            {
                context.Result = new StatusCodeResult(StatusCodes.Status200OK);

            }
        }
    }

}
