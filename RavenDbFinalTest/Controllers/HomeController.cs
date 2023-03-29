using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RavenDbFinalTest.Models;
using System.Diagnostics;
using Raven.Client.Documents;
using RavenDbFinalTest.Models;
using System.Linq;
using System.Reflection;
using Raven.Client.Documents;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;


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
            try
            {

                HttpClient client = new HttpClient();
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
                return Content(otp + "," + email + "@ceiamerica.com");
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        public int Otp { get; set; }
        [HttpPost]
        public async Task<IActionResult> LoginAuth(string otp, string email2, string userotp)
        {
            if (otp == userotp)
            {
                var certificate = new X509Certificate2("Cloud.pfx", "93EE9D996433A0E1B61FF03749B2AFC7");

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
                        var employee = session.Query<LoginData>(collectionName: "Companies").FirstOrDefault(u => u.EmailId == email2);
                        if (employee != null)
                        {
                            ViewBag.EmailExists = true;
                            if (employee.role == "Admin")
                            {
                                ViewBag.RoleMessage = $"{employee.Name}, You are Admin";

                            }
                            else if(employee.role == "Employee")
                            {

                                var login = new Login()
                                {
                                    EmailAddress = email2,
                                    LoginTime = DateTime.UtcNow,
                                    IpAddress = HttpContext.Connection.RemoteIpAddress.ToString()

                                };
                                session.Store(login, login.Id);
                                session.SaveChanges();
                                ViewBag.RoleMessage = $"{employee.Name}, You are Employee";
                            }
                        }
                        else
                        {
                            ViewBag.EmailExists = false;
                        }

                    }

                   

                }
                //karuppaiah code begins
                HttpClient client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5000/getcred");
                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("email", email2) });
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
                return View("~/Views/Home/LoginAuth.cshtml");
                /* return new ContentResult
                 {
                     Content = "Hi " + email2+" Login successfull" + myValue,
                     ContentType = "text/plain",
                     StatusCode = 200
                 };*/
            }
            else
            {
                return new ContentResult
                {
                    Content = "Enter valid otp",
                    ContentType = "text/plain",
                    StatusCode = 200
                };
            }

        }
        [HttpPost]
        public ActionResult CheckEmail(string emailAddress)
        {
            var certificate = new X509Certificate2("Cloud.pfx", "93EE9D996433A0E1B61FF03749B2AFC7");

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
    
public async Task<IActionResult> LoginAuth()
        {
            string Token = HttpContext.Session.GetString("cToken");
            if (Token != null)
            {
                return View();
            }
            else
            {
                return Redirect("Index");
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