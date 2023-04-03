using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RavenDbFinalTest.Models;
using System.Diagnostics;
using Raven.Client.Documents;
using RavenDbFinalTest.Models;
using Raven.Client.Documents;



using GraphQL.Client.Http;
using GraphQL;
using GraphQL.Client.Serializer.Newtonsoft;
using System.Security.Cryptography;
using GraphQL.Validation;

namespace RavenDbFinalTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseURL = "http://localhost:5000";
        public int globaleid;
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
              var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://authentication-nodejs-ettd.onrender.com") }, new NewtonsoftJsonSerializer());
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
            globaleid = user.eid;
            Console.WriteLine(globaleid);
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

               
                   
                }


            //New code begins

            var client = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://authentication-nodejs-ettd.onrender.com") }, new NewtonsoftJsonSerializer());

            var Graphlogauth = new GraphQLRequest
            {
                Query = @"query($userotp: Int){
                  cred(userotp: $userotp)
                }",
                Variables = new { userotp = userotp }
            };
            var logauthres = await client.SendQueryAsync<dynamic>(Graphlogauth);
            string ctoken = logauthres.Data.cred;
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

        public async Task<IActionResult> profile(string id,bool success= false)
        {
            int eid = Int16.Parse(id);
          // int usereid =Int16.Parse( HttpContext.Session.GetString("usereid"));
            if (true)
            {
                if (success)
                {
                    ViewBag.successmessage = "Form data successfully updated!";
                }
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

        [HttpPost]
        public async Task<ActionResult> Edit(Profile2 model)
        {
            Console.WriteLine("Hi here"+model.FirstName);
            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions
            { EndPoint = new Uri("https://localhost:7000/graphql") },
            new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @"mutation example($test:Profile2Input!){
                  editdata (data: $test){
                    firstName
                  }
                }",
                Variables = new
                {
                    test = new
                    {
                        model.FirstName,
                        model.LastName,
                        model.Department,
                        model.Manager,
                        model.Job_Title,
                        model.Email,
                        model.Address,
                        model.Phone_Number


                    }
                }
            };
            Console.WriteLine("Before");
            await client2.SendQueryAsync<dynamic>(graphqlreq);
            return RedirectToAction("Profile", new { id = globaleid, success = true });
        }

        public async Task<IActionResult> GetImage(string id)
        {
            int oid = Int16.Parse(id);
            Console.WriteLine("this is oid" + oid + "eid:" + id);
            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @"query example($id:Int!){
                  getimage(id: $id) {
                    firstName
                    imageBase64
                    id
                  }
                }",
                Variables = new { id = oid }
            };
            var req = await client2.SendQueryAsync<dynamic>(graphqlreq);
            var user = req.Data.getimage;
            Console.WriteLine(user.firstName);
            Console.WriteLine(user.imageBase64.GetType().Name);

            Console.WriteLine("This is from getprofile userid:"+user.id);
            if (user.imageBase64 != null)
            {
                 // Convert to string
                 var imageDataString=Convert.ToBase64String(user.imageBase64);
                var imageData = Convert.FromBase64String(imageDataString);
                var stream = new MemoryStream(imageData);
                return new FileStreamResult(stream, "image/jpeg");

            }
            else
            {
                return NotFound();
            }

        }
        //This is to getimage

        /*  public IActionResult GetImage()
          {
              var certificate = new X509Certificate2("Cloud.pfx", "93EE9D996433A0E1B61FF03749B2AFC7");
              //var certificate = new X509Certificate2("certificate.pfx", "password");

              using (var store = new DocumentStore
              {
                  Urls = new[] { " https://a.free.rmanojcei.ravendb.cloud/" },
                  Database = "TestEmployee",
                  Certificate = certificate
              })
              {
                  store.Initialize();

                  using (var session = store.OpenSession())
                  {
                      var attachment = session.Advanced.Attachments.Get("4065d794-2c10-4d45-bf8f-b8befb9d1797", "mathew.jpg");
                      if (attachment != null)
                      {
                          var imageStream = new MemoryStream();
                          attachment.Stream.CopyTo(imageStream);

                          return File(imageStream.ToArray(), "image/jpeg"); // Replace "image/jpeg" with the appropriate content type
                      }
                      else
                      {
                          Console.WriteLine("Not ound");
                          return NotFound();
                      }
                  }
              }

          }
        */


        /*
         string imageDataString = user.imageBase64.ToString(); // Convert to string
                var imageData = Convert.FromBase64String(imageDataString);

                // Save the image to a file
                using (var fileStream = new FileStream("test.jpg", FileMode.Create, FileAccess.Write))
                {
                    await fileStream.WriteAsync(imageData, 0, imageData.Length);
                }

                // Return the image as a FileStreamResult
                var fileStreamResult = new FileStreamResult(new MemoryStream(imageData), "image/jpeg");
                fileStreamResult.FileDownloadName = "test.jpg";
                return fileStreamResult;
         
         */

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