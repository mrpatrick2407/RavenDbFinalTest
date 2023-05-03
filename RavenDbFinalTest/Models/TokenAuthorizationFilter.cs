using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net.Http.Headers;

namespace RavenDbFinalTest.Models
{
    public class TokenAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string _token;

        public TokenAuthorizationFilter(string token)
        {
            _token = token;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var headers = context.HttpContext.Request.Headers;
            if (!headers.ContainsKey("Authorization"))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var authHeader = AuthenticationHeaderValue.Parse(headers["Authorization"]);
            if (authHeader.Scheme != "Bearer")
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = authHeader.Parameter;
            // compare token with the one stored in the filter
            if (!string.Equals(token, _token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }

}
