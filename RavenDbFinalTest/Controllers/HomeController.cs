using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RavenDbFinalTest.Models;
using System.Diagnostics;
using Raven.Client.Documents;
using RavenDbFinalTest.Models;


using GraphQL.Client.Http;
using GraphQL;
using GraphQL.Client.Serializer.Newtonsoft;


namespace RavenDbFinalTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseURL = "http://localhost:5000";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
            
        }
        [HttpPost]

        public async Task<IActionResult> Index(String email)
        {
              var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("http://localhost:4000/graphql") }, new NewtonsoftJsonSerializer());
                var graphqlreq = new GraphQLRequest
                {
                        Query = @"query($email: String){
                          SendEmailQuery(email: $email)
                        }",
                    Variables = new { email = email+"@ceiamerica.com" }
                };
                var res = await client2.SendQueryAsync<dynamic>(graphqlreq);
            Console.WriteLine(res);
                var SendEmailRes = res.Data.SendEmailQuery;
                if (SendEmailRes == "error")
                {
                    return Content("Something went wrong");
                }
                else
                {
                    return Content(email + "@ceiamerica.com");
                }
                /*  HttpClient client = new HttpClient();
                  var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5000/sendemail");
                  var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("emailID", email + "@ceiamerica.com") });
                  request.Content = content;

                  HttpResponseMessage response = await client.SendAsync(request);
                  response.EnsureSuccessStatusCode();
                  // Deserialize the response body to a JSON object
                  string responseBody = await response.Content.ReadAsStringAsync();


                  var json = JsonSerializer.Deserialize<JsonElement>(responseBody);

                  string otp = json.GetProperty("message").GetString();

                  ViewBag.Otp = otp;
                  TempData["otp"] = otp;
                  TempData["email"] = email + "@ceiamerica.com";
                  ViewBag.Email = email + "@ceiamerica.com";
                  return Content(otp + "," + email + "@ceiamerica.com");*/

//Old code ends here

                /*return new ContentResult
                {
                    Content = otp,
                    ContentType = "text/plain",
                    StatusCode = 200
                };*/
                /*string otp = json.GetProperty("otp").GetString();
                HttpContext.Session.SetString("otp", otp);
                string myValue = HttpContext.Session.GetString("MyValue");
                Console.WriteLine("hi");*/


                /*if (Otp3.ToString()!= null)
                {
                    string otp = json.GetProperty("otp").GetString();
                    if (Otp3.ToString() == otp)
                    {
                        return View("LoginAuth");
                    }
                }*/

                // Extract the OTP value from the JSON object
                //string otp = json.GetProperty("otp").GetString();
                //HttpContext.Session.SetString("OTP", otp);

                // Extract the OTP value from the JSON object
                //string otp = jsonObject.GetProperty("otp").GetString();


           
        }

        [HttpPost]
        public async Task<IActionResult> Emailcheck(string email)
        {
            try
            {
                var originalemail = email + "@ceiamerica.com";
                var client = new GraphQLHttpClient(new GraphQLHttpClientOptions
                {
                    EndPoint = new Uri("https://localhost:7000/graphql")
                }, new NewtonsoftJsonSerializer());

                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"query($email: String!) {
                emailExists(email: $email)
            }",
                    Variables = new { email = originalemail }
                };

                var graphQLResponse = await client.SendQueryAsync<dynamic>(graphQLRequest);
                bool emailExistsInGraphQL = graphQLResponse.Data.emailExists;
                
                return Json(emailExistsInGraphQL);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
                return BadRequest();
            }
        }

 /*       public async Task<IActionResult> Verygood(string otp,string email2,string userotp)
        {
            var client = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint =new Uri("https://localhost:7000/graphql" )}, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest {
                Query= @"query ExampleQuery($email:String!){
             roleCheck(email: $email)
            }",Variables=new { email = email2 }
            };
            var res = await client.SendQueryAsync<dynamic>(graphqlreq);
            Console.WriteLine(res);
            return Json(res);
        }
 */
        public int Otp { get; set; }
        [HttpPost]
        public async Task<IActionResult> LoginAuth( string email2, int userotp)
        {
            
                var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
                var graphqlreq = new GraphQLRequest
                {
                    Query = @"query example($email:String!){
                  getemployee(email: $email) {
                  emailId
                  role
                  name
                eid
                  }
                }",
                    Variables = new { email = email2 }
                };
                var res = await client2.SendQueryAsync<dynamic>(graphqlreq);
                var user = res.Data.getemployee;
            
                string username = user.name;
                string usereid = user.eid;
                string emailid=user.emailId;
                HttpContext.Session.SetString("username", username);
                HttpContext.Session.SetString("usereid", usereid);
                HttpContext.Session.SetString("useremailid", emailid);

            if (user.role == "Admin")
                {
                    ViewBag.RoleMessage = $"{user.Name}, You are Admin";
                }else if (user.role == "Employee")
                {
                   
                    
                    ViewBag.RoleMessage = $"{user.Name}, You are Employee";
                    var loginactivity = new GraphQLRequest
                    {
                        Query = @"mutation example($email:String!){
                  savelogin(email: $email) {
                    
                    loginTime
                    
                  }
                }",
                        Variables = new { email = email2}
                    };
                    var logres =await client2.SendQueryAsync<dynamic>(loginactivity);
                    var reponse = logres.Data.savelogin;
                    Console.WriteLine(reponse);

               
                   
                }


            //New code begins

            var client = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("http://localhost:4000/graphql") }, new NewtonsoftJsonSerializer());

            var Graphlogauth = new GraphQLRequest
            {
                Query = @"query($userotp: Int){
                  cred(userotp: $userotp)
                }",
                Variables = new { userotp = userotp }
            };
            var logauthres = await client.SendQueryAsync<dynamic>(Graphlogauth);
            string ctoken = logauthres.Data.cred;
            Console.WriteLine($"{ctoken}");
            if(ctoken == "Invalid OTP")
            {
                Console.WriteLine("If");
                return Content(ctoken);
                
            }
            else
            {
                Console.WriteLine("Else");
                HttpContext.Session.SetString("cToken", ctoken);
                string myValue = HttpContext.Session.GetString("cToken");
                return Content("Valid OTP");

            }
           


            /*    //karuppaiah old code begins
                HttpClient client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5000/getcred");
                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("email", email2), new KeyValuePair<string, string>("otp", userotp) });
                request.Content = content;

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                // Deserialize the response body to a JSON object
                string responseBody = await response.Content.ReadAsStringAsync();
                

                var json = JsonSerializer.Deserialize<JsonElement>(responseBody);
                string CustomToken = json.GetProperty("customToken").GetString();
                //DateTime expiryTime = DateTime.Now.AddSeconds(50);
                //HttpContext.Session.Set("cToken", BitConverter.GetBytes(expiryTime.Ticks));

                HttpContext.Session.SetString("cToken", CustomToken);
                string myValue = HttpContext.Session.GetString("cToken");
                return Content("Valid OTP");*/
            /* return new ContentResult
             {
                 Content = "Hi " + email2+" Login successfull" + myValue,
                 ContentType = "text/plain",
                 StatusCode = 200
             };*/



        }

        /*
        [HttpPost]
        public ActionResult CheckEmail(string emailAddress)
        {
            var certificate = new X509Certificate2("ClientCertificate.pfx", "Theophilus");

            using (var store = new DocumentStore
            {
                Urls = new[] { " https://a.theophilus.ravendb.community" },
                Database = "TestEmployee",
                Certificate = certificate
            })
            {
                store.Initialize();


                using (var session = store.OpenSession())
                {
                    var employee = session.Query<LoginData>(collectionName: "Companies").FirstOrDefault(u => u.EmailId == emailAddress);
                    if (employee != null)
                    {
                        ViewBag.EmailExists = true;
                        if (employee.role == "Admin")
                        {
                            ViewBag.RoleMessage = $"{employee.Name} is Admin";

                        }
                        else
                        {
                            var login = new Login()
                            {
                                EmailAddress = emailAddress,
                                LoginTime = DateTime.UtcNow,
                                IpAddress = HttpContext.Connection.RemoteIpAddress.ToString()

                            };
                            session.Store(login, login.Id);
                            session.SaveChanges();
                            ViewBag.RoleMessage = $"{employee.Name} is Employee";
                        }
                    }
                    else
                    {
                        ViewBag.EmailExists = false;
                    }

                }

                return View();

            }
        }
    */
       
        public async Task<IActionResult> LoginAuth(string email2)
        {   
            try
            {

                    string Token = HttpContext.Session.GetString("cToken");
                    string useremail= HttpContext.Session.GetString("useremailid");
                    if(Token!=null && useremail==email2)
                    {
                        var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
                        var graphqlreq = new GraphQLRequest
                        {
                            Query = @"query example($email:String!){
                              getemployee(email: $email) {
                              emailId
                              role
                              name
                              eid
                              }
                            }",
                            Variables = new { email = email2 }
                        };
                        var res = await client2.SendQueryAsync<dynamic>(graphqlreq);
                        var user = res.Data.getemployee;
                        
                        return View();
                    }
                else
                {
                    return View("Error");
                }
                
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Redirect("Index");
            }
            
        }

        public async Task<IActionResult> profile(string id)
        {
            int eid = Int16.Parse(id);
            Console.WriteLine(eid);
            int usereid =Int16.Parse( HttpContext.Session.GetString("usereid"));
            if (usereid==eid)
            {
                Console.WriteLine("From profile inside");
                var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
                var graphqlreq = new GraphQLRequest
                {
                    Query = @"query exaple($id:Int!){
                 getemployeebyid(id: $id) {
                   firstName
                   lastName
                   phone_Number
                   address
                   job_Title
                   manager
                   department
                   email
                   id
                 }
                }",
                    Variables = new { id = eid }
                };
                var req = await client2.SendQueryAsync<dynamic>(graphqlreq);
                var user = req.Data.getemployeebyid;
                return View(user);
            }
            else
            {
                return View("Error");
            }
            
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("cToken");
            return Redirect("/");

        }
        public IActionResult EmployeeHome()
        {
            string myValue = HttpContext.Session.GetString("cToken");

            if (myValue != null)
            {
                return View("~/Views/Home/EmployeeHome.cshtml");
            }
            else
            {
                return View("Index");
            }

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    public class MyClass
    {
        public string otp { get; set; }

    }
}