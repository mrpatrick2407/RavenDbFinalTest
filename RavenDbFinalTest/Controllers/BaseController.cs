using GraphQL.Client.Http;
using Microsoft.AspNetCore.Mvc;
using GraphQL;
using GraphQL.Client.Http;

using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;

namespace RavenDbFinalTest.Controllers
{
    public class BaseController : Controller
    {
        public async Task<String?> ValidateToken(string? email,int? eid)
        {
            if (eid != null)
            {
                var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
                var graphqlreq = new GraphQLRequest
                {
                    Query = @"query exaple($id:Int!){
                 getemployeebyid(id: $id) {
                email
                 }
                }",
                    Variables = new { id = eid }
                };
                var req1 = await client2.SendQueryAsync<dynamic>(graphqlreq);



                email = req1.Data.getemployeebyid.email;

            }
            var token =HttpContext.Session.GetString("ctoken");
            var client = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("http://localhost:4000") }, new NewtonsoftJsonSerializer());



            var req = new GraphQLRequest
            {
                Query = @"query Query($token: String!, $email: String!) {
                  validatetoken(token: $token, email: $email)
                }",
                Variables = new { token = token, email = email }
            };
            var res = await client.SendQueryAsync<dynamic>(req);
            
            string isValid = res.Data.validatetoken;
            Console.WriteLine(isValid);

            return isValid;
        }
    }
}
