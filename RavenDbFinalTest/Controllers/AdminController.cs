using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RavenDbFinalTest.Models;
using System.Diagnostics;
using Raven.Client.Documents;
using RavenDbFinalTest.Models;
using GraphQL.Client.Http;
using GraphQL;
using GraphQL.Client.Serializer.Newtonsoft;
using static Raven.Client.Http.ServerNode;
using System.Security.Cryptography;
using Elastic.Clients.Elasticsearch;
using System;
using Nest;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace RavenDbFinalTest.Controllers
{
    public class AdminController : Controller
    {
        private readonly ElasticsearchClient _client;

        public AdminController(ElasticsearchClient client)
        {
            _client = client;
        }
        /*public async Task<IActionResult> Search()
        {
            /* Arrange
            var tweet = new Tweet
            {
                Id = 1,
                User = "stevejgordon",
                PostDate = new DateTime(2009, 11, 15),
                Message = "Trying out the client, so far so good?"
            };

            var response = await _client.IndexAsync(tweet, "Companies");

            if (response.IsValidResponse)
            {
                Console.WriteLine($"Index document with ID {response.Id} succeeded.");
            }
            return View();
            */
        /*var tweets = new List<Tweet>
            {
                new Tweet
                {
                    Id = 1,
                    User = "afsdffdz",
                    PostDate = new DateTime(2009, 11, 15),
                    Message = "Trying out the client, so far so good?"
                },
                new Tweet
                {
                    Id = 2,
                    User = "erers",
                    PostDate = new DateTime(2010, 6, 20),
                    Message = "sdfsdf is cool"
                },
                new Tweet
                {
                    Id = 7,
                    User = "stordon",
                    PostDate = new DateTime(2012, 2, 3),
                    Message = "Writing a blog post about Elasticsearch"
                }
            };

        List<Company> model = new List<Company>();
        var certificate = new X509Certificate2("Cloud.pfx", "93EE9D996433A0E1B61FF03749B2AFC7");

        using (var store = new DocumentStore
        {
            Urls = new[] { "https://a.free.rmanojcei.ravendb.cloud" },
            Database = "TestEmployee",
            Certificate = certificate
        })
        {
            store.Initialize();

            using (var session = store.OpenSession())
            {

                model = session.Query<Company>(collectionName: "Companies").ToList();
            }
        }
        var response = await _client.IndexManyAsync(model, "company");

        if (response.IsValidResponse)
        {
            Console.WriteLine($"Index document with ID {response.Items.First().Id} succeeded.");
        }
        return View();
    }*/

        public IActionResult CreateEmployee()
        {
            return View();

        }



        [HttpPost]
        public async Task<IActionResult> CreateEmployee(int Eid, string Name, string role, string Phone, string EmailId)
        {
            try
            {
                var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
                var graphqlreq = new GraphQLRequest
                {
                    Query = @" mutation createEmployee($eid: Int!, $name: String!, $role: String!, $phone: String!, $emailId: String!) {
                              createEmployee(eid: $eid, name: $name, role: $role,  phone: $phone, emailId: $emailId) {
                               
                                name
                              }
                            }
                            ",
                    Variables = new
                    {
                        eid = Eid,

                        name = Name,
                        role = role,

                        phone = Phone,
                        emailId = EmailId
                    }
                };
                var res = await client2.SendQueryAsync<dynamic>(graphqlreq);
                TempData["SuccessMessage"] = "Employee created successfully";
            }
            catch (Exception ex)
            {
                TempData["SuccessMessage"] = "Something went wrong";
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Action(int id, bool check)
        {
            if (id == null || id == 0)
            {
                return BadRequest();
            }
            Console.WriteLine(id.GetType().Name + " " + id);
            Console.WriteLine(check.GetType().Name + " " + check);

            //var fid = Int16.Parse(id);
            var client = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLHttpRequest
            {
                Query = @"query example ($eid:Int!,$val:Boolean!){
                  getemployeereq(id: $eid,val: $val)
                }",
                Variables = new { eid = id, val = check }
            };
            try
            {
                await client.SendQueryAsync<dynamic>(graphqlreq);
                return Ok();

            }
            catch (Exception e)
            {

                return BadRequest();
            }


        }
        public async Task<IActionResult> AdminRequest()
        {
            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @"query{
                    allemployeereq{
                    firstName
                    lastName
                    department
                    manager
                    address
                    phone_Number
                    job_Title
                    email
                    eid
                    id
                  }
                }",

            };
            var res = await client2.SendQueryAsync<dynamic>(graphqlreq);
            var requests = res.Data.allemployeereq.ToObject<List<AdminRequest>>();

            return View(requests);
        }


       
        public async Task<IActionResult> ListEmployee()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // Return a 400 Bad Request response if the Authorization header is missing
                return BadRequest("Authorization header is missing");
            }

            string idToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJodHRwczovL2lkZW50aXR5dG9vbGtpdC5nb29nbGVhcGlzLmNvbS9nb29nbGUuaWRlbnRpdHkuaWRlbnRpdHl0b29sa2l0LnYxLklkZW50aXR5VG9vbGtpdCIsImlhdCI6MTY4MjUwNjc1NCwiZXhwIjoxNjgyNTEwMzU0LCJpc3MiOiJmaXJlYmFzZS1hZG1pbnNkay11ZTA4aEBmaXItYXV0aC1kZDhjZS5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsInN1YiI6ImZpcmViYXNlLWFkbWluc2RrLXVlMDhoQGZpci1hdXRoLWRkOGNlLmlhbS5nc2VydmljZWFjY291bnQuY29tIiwidWlkIjoiVldzMjBkdjhrcmIyMHgxSk5hVTRXRno3MHk4MyJ9.AFyubXdoKvbUx6y5QtAf9I8jognwJW3OJye_K4WMq1vGNQau-wAZWykRth-bHLUoiaewe0HdheHtyi4WRFLbFWav5qBdwmR2fqBZRyIeawgrKeXEqjH_R0XRm_FLRXPVgB2hj1vcKAY4IZXtaubSZYr4vzgqQ0wg8j8gJ6dYtmDPxB9Sn60EKpKa1LlLlC2Hyp6KCw0_fXXusBS2ZtbOHYTfYtlb7NIqf1FUlAS_QpDFf4k4PhnaeikFyjh_jTauYDLxk-PyrjulgDIv3F0g4so-n1eYGIxzVpPikp0sAxdiiIilEu332ZF-CEULQ-r6BwtQv6yHQeGVJ7L2RaQO1w";

            // Decode the ID token
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(idToken);

            // Get the user's email address from the token's claims
            string userEmail = token.Claims.FirstOrDefault(c => c.Type == "iat")?.Value;
            Console.WriteLine("Useremail" + userEmail);
            var client = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLHttpRequest
            {
                Query = @"query{
                  pushtoelastic
                }",
               
            };
            try
            {
                await client.SendQueryAsync<dynamic>(graphqlreq);
                

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
            Console.WriteLine($"Index document with ID  succeeded.");
            return View();
        }
            
    

        public async Task<IActionResult> EditEmployee(string Id)
        {
            Console.WriteLine("This is edit" + Id);
            
            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @" query example($id:String!){
                  getemployeebyidadmin(id: $id) {
                  emailId
                  eid
                  role
                  name
                  phone
                  }
                } ",
                Variables = new
                {
                    id = Id,

    
                }
            };
            var res = await client2.SendQueryAsync<dynamic>(graphqlreq);
            var Employee = res.Data.getemployeebyidadmin;
            ViewBag.name = Employee.name;
            ViewBag.emailId = Employee.emailId;
            ViewBag.phone = Employee.phone;
            ViewBag.role = Employee.role;
            ViewBag.id = Id;
            ViewBag.eid = Employee.eid;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EditEmployee(string id, int Eid, string Name, string role, string Phone, string EmailId)
        {
            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @" mutation editEmployee($eid: Int!, $name: String!, $role: String!, $phone: String!, $emailId: String!,$id:String!) {
                              editEmployee(eid: $eid, name: $name, role: $role,  phone: $phone, emailId: $emailId,id:$id) {
                                name
                              }
                            }
                            ",
                Variables = new
                {
                    eid = Eid,
                    id = id,
                    name = Name,
                    role = role,

                    phone = Phone,
                    emailId = EmailId
                }
            };
            var res = await client2.SendQueryAsync<dynamic>(graphqlreq);

            return Redirect("/Admin/ListEmployee");

        }


        public async Task<IActionResult> DeleteEmployeeById(int Eid)
        {
            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @" mutation deleteEmployeeById($eid: Int !) {
                              deleteEmployeeById(eId: $eid){
                              emailId
                              }
                            }
                            ",
                Variables = new
                {
                    eid = Eid,


                }
            };
            var res = await client2.SendQueryAsync<dynamic>(graphqlreq);

            return Redirect("/Admin/ListEmployee");

        }

    }
}
