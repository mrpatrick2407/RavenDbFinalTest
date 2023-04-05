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

namespace RavenDbFinalTest.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
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


        public async Task<IActionResult> ListEmployee()
        {
            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @"query {
                              allEmployees {
                                id
                                name
                                role
                                phone
                                emailId
                                eid

                              }
                            }",

            };
            var res = await client2.SendQueryAsync<dynamic>(graphqlreq);
            var Employee = res.Data.allEmployees.ToObject<List<Company>>();

            return View(Employee);
        }


        public async Task<IActionResult> EditEmployee(string Id)
        {
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
