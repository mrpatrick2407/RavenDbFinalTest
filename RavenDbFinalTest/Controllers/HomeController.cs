using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RavenDbFinalTest.Models;
using System.Diagnostics;
using Raven.Client.Documents;
using RavenDbFinalTest.Models;
using Raven.Client.Documents;
using System.IdentityModel.Tokens.Jwt;

using GraphQL.Client.Http;
using GraphQL;
using GraphQL.Client.Serializer.Newtonsoft;
using System.Security.Cryptography;
using GraphQL.Validation;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.SignalR;
using System.Drawing;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication;
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
            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://new-autentication.onrender.com/") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @"query($email: String){
                          SendEmailQuery(email: $email)
                        }",
                Variables = new { email = email + "@ceiamerica.com" }
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
        public async Task<IActionResult> Notifications(int id, DateTime timestamp)
        {
            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @"query example($id:Int!,$timestamp:DateTime!){
          getNotificationsByIdAndTimestamp(id: $id,timestamp: $timestamp){
            message
            timestamp
            id
            status
          }
        }",
                Variables = new { id = id, timestamp = timestamp }
            };
            var res = await client2.SendQueryAsync<dynamic>(graphqlreq);
            Console.WriteLine(res.Data.getNotificationsByIdAndTimestamp);
            var messagesList = new List<string>();
            var idList = new List<string>();
            var statusList = new List<bool>();

            foreach (var notification in res.Data.getNotificationsByIdAndTimestamp)
            {
                string message = notification.message;
                messagesList.Add(message);
                string ids = notification.id;
                idList.Add(ids);
                bool status1 = notification.status;
                statusList.Add(status1);
            }

            var messages = messagesList.ToArray();
            var list = idList.ToArray();
            var status = statusList.ToArray();

            return Json(new { messages, list, status });
        }

        public async Task<IActionResult> NotiDelete(string id)
        {
            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @"mutation example($id: String!) {
                 deletenotificationbyid(id: $id) {
                 message
                 }
                }",
                Variables = new { id = id }
            };
            var res = await client2.SendQueryAsync<dynamic>(graphqlreq);


            return Content("yes");

        }
        public async Task<IActionResult> NewNotifications(int id, DateTime timestamp)
        {
            Console.WriteLine("NOtifcation" + id + "ts" + timestamp.ToString());
            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @"query example($id:Int!,$timestamp:DateTime!){
                newnotification(id: $id,timestamp: $timestamp){
                message
                timestamp
                id
                status
                }
                }",
                Variables = new { id, timestamp }
            };
            var res = await client2.SendQueryAsync<dynamic>(graphqlreq);
            Console.WriteLine(res.Data.newnotification);
            var messagesList = new List<string>();
            var idList = new List<string>();
            var statusList = new List<bool>();

            foreach (var notification in res.Data.newnotification)
            {
                string message = notification.message;
                messagesList.Add(message);
                string ids = notification.id;
                idList.Add(ids);
                bool status1 = notification.status;
                statusList.Add(status1);
            }

            var messages = messagesList.ToArray();
            var list = idList.ToArray();
            var status = statusList.ToArray();

            return Json(new { messages, list, status });
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



        public int Otp { get; set; }
        [HttpPost]
        public async Task<IActionResult> LoginAuth(string email2, int userotp)
        {
            string suserotp = userotp.ToString();
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
            string emailid = user.emailId;
            globaleid = Int16.Parse(usereid);

            Console.WriteLine("This is global" + globaleid);
            HttpContext.Session.SetString("username", username);
            HttpContext.Session.SetString("usereid", usereid);
            HttpContext.Session.SetString("useremailid", emailid);

            if (user.role == "Admin")
            {
                ViewBag.RoleMessage = $"{user.Name}, You are Admin";
            }
            else if (user.role == "Employee")
            {


                ViewBag.RoleMessage = $"{user.Name}, You are Employee";
                var loginactivity = new GraphQLRequest
                {
                    Query = @"mutation example($email:String!){
                  savelogin(email: $email) {
                    
                    loginTime
                    
                  }
                }",
                    Variables = new { email = email2 }
                };
                var logres = await client2.SendQueryAsync<dynamic>(loginactivity);
                var reponse = logres.Data.savelogin;



            }


            //New code begins

            var client = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://new-autentication.onrender.com/") }, new NewtonsoftJsonSerializer());

            var Graphlogauth = new GraphQLRequest
            {
                Query = @"query($userotp: String, $email: String){
                  cred(userotp: $userotp, email: $email)
                }",
                Variables = new { userotp = suserotp, email = email2 }
            };
            var logauthres = await client.SendQueryAsync<dynamic>(Graphlogauth);
            string ctoken = logauthres.Data.cred;
            if (ctoken == "Invalid OTP" || ctoken == "Expired OTP")
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
        [HttpPost]
        public async Task<IActionResult> UploadImage(string imageFile)
        {
            //professionalism check code begins
            /* byte[] imageBytes = Convert.FromBase64String(imageFile);

             var file = new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, "Image", "image.jpg")
             {
                 Headers = new HeaderDictionary(),
                 ContentType = "image/jpeg"
             };
             if (file == null || file.Length == 0)
             {
                 return BadRequest("Please select an image file");
             }



             var fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
             var filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);



             using (var stream = new FileStream(filePath, FileMode.Create))
             {
                 await file.CopyToAsync(stream);
             }



            var imageUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/images/{fileName}";

             var httpClient = new HttpClient();



             // set the authorization header
             var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("pJ6eK0PrXOeT4nzhhbmCYOMJ:zz5iH6llonMGmV02WrThUfYz719nfqoMZbmLTbmFuiLtApZ4"));
             httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
             Console.WriteLine("Credentials  " + credentials+"   ");


             // send the request using HttpClient
             var response = await httpClient.GetAsync("https://api.everypixel.com/v1/quality_ugc?url=https://cdn.pixabay.com/photo/2015/04/23/22/00/tree-736885__480.jpg");



             // read the response content as a string
             var responseContent = await response.Content.ReadAsStringAsync();
             Console.WriteLine(responseContent);
            // Console.WriteLine(imageUrl);
             return Content(responseContent);
            //professionalism check code ends
            // construct the image file from the Base64-encoded image data
            using (var client = new HttpClient())
            {
                var username = "pJ6eK0PrXOeT4nzhhbmCYOMJ";
                var password = "zz5iH6llonMGmV02WrThUfYz719nfqoMZbmLTbmFuiLtApZ4";
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                var imageBytes = Convert.FromBase64String(imageFile);

                using (var content = new MultipartFormDataContent())
                {
                    var imageContent = new ByteArrayContent(imageBytes);
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    content.Add(imageContent, "image", "image.jpg");

                    var response = await client.PostAsync("https://api.everypixel.com/v1/keywords", content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(responseString);
                    return Content(responseString);
                }
            }
            */


            // Console.WriteLine("Image file" + imageFile);


            //graphqlquery
            int eid = Int16.Parse(HttpContext.Session.GetString("usereid"));

            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @"query exaple($id:Int!){
                 getemployeebyid(id: $id) {
                   id
                 }
                }",
                Variables = new { id = eid }
            };
            var req = await client2.SendQueryAsync<dynamic>(graphqlreq);
            var user = req.Data.getemployeebyid;
            string userid = user.id.ToString();
            Console.WriteLine("useridofab" + userid);

            /*         string imageString = "";
                          using (var memoryStream = new MemoryStream())
                          {
                              await imageFile.CopyToAsync(memoryStream);
                              byte[] imageBytes = memoryStream.ToArray();
                              imageString = Convert.ToBase64String(imageBytes);
                          }

              */
            //checkimage
            var graphclient = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());

            Console.WriteLine(" checek begin");
            // byte[] imageBytes = Convert.ToByte(imageFile);
            var imagecheck = new GraphQLRequest
            {
                Query = @"mutation example($image:String!){
              CheakImage(image: $image)
            }",

                Variables = new { image = imageFile }

            };

            var response = await graphclient.SendQueryAsync<dynamic>(imagecheck);

            string wholescore = response.Data.CheakImage;

            JObject json = JObject.Parse(wholescore);
            double score = (double)json["quality"]["score"];
            string Status = "";
            if (score < 0.6)
            {
                //ViewBag.CheckImage = "Try to upload proper image";
                Status = "Try uploading proper Image";
            }
            Console.WriteLine(score);
            Console.WriteLine("checek end");

            var imageuploadreq = new GraphQLRequest
            {
                Query = @"mutation example($image:String!,$userid:String!){
                  storeImageAsync(image: $image,userid :$userid)
                }",
                Variables = new { image = imageFile, userid = userid }
            };

            await client2.SendQueryAsync<dynamic>(imageuploadreq);
            if (Status != "")
            {

                return Content(Status);
            }
            return Ok();






        }





        public async Task<IActionResult> LoginAuth(string email2)
        {
            try
            {
                string Token = HttpContext.Session.GetString("cToken");
                string useremail = HttpContext.Session.GetString("useremailid");
                if (Token != null && useremail == email2)
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

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Redirect("Index");
            }

        }

        public async Task<IActionResult> profile(string id)
        {
            int eid = Int16.Parse(id);
            // int usereid =Int16.Parse( HttpContext.Session.GetString("usereid"));
            if (true)
            {

                var client = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("https://localhost:7000/graphql") }, new NewtonsoftJsonSerializer());
                var request = new GraphQLRequest
                {
                    Query = @"query example($id:Int!){
                          getadminreq(id: $id) 
                        }",
                    Variables = new { id = eid }
                };
                var response = await client.SendQueryAsync<dynamic>(request);
                bool requestsexist = response.Data.getadminreq;
                // Console.WriteLine("Idhu vandhu inga request"+requestsexist);
                if (requestsexist)
                {
                    ViewBag.successmessage = "Requested!";
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
            string eid = HttpContext.Session.GetString("usereid");
            int eid2 = Int16.Parse(eid);
            model.eid = eid2;
            Console.WriteLine("Eid from EDit" + eid);
            var client2 = new GraphQLHttpClient(new GraphQLHttpClientOptions
            { EndPoint = new Uri("https://localhost:7000/graphql") },
            new NewtonsoftJsonSerializer());
            var graphqlreq = new GraphQLRequest
            {
                Query = @" mutation example($test:Profile2Input!){
                editdata (data: $test){
                firstName
                }
                }",
                Variables = new
                {
                    test = new
                    {
                        model.eid,
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
            return RedirectToAction("Profile", new { id = eid });
        }

        public async Task<IActionResult> GetImage(string id)
        {
            int oid = Int16.Parse(id);
            //Console.WriteLine("this is oid" + oid + "eid:" + id);
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
            //Console.WriteLine(user.firstName);
            //Console.WriteLine(user.imageBase64.GetType().Name);


            if (user.imageBase64 != null)
            {
                // Convert to string
                var imageData = Convert.FromBase64String(user.imageBase64.ToString());

                var stream = new MemoryStream(imageData);
                return new FileStreamResult(stream, "image/jpeg");

            }
            else
            {
                return NotFound();
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