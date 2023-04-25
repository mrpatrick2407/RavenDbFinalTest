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
