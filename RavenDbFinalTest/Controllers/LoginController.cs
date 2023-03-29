using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using RavenDbFinalTest.Models;
using System.Linq;
using System.Reflection;
using Raven.Client.Documents;
using System.Security.Cryptography.X509Certificates;

namespace RavenDbFinalTest.Controllers
{
    public class LoginController : Controller
    {

        public ActionResult Index()
        {
            return View();
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
                            session.Store(login,login.Id);
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
    }
}


/*
      [HttpPost]
        public ActionResult CheckEmail(string emailAddress)
        {
            var certificate = new X509Certificate2("C:\\Users\\Manoj\\source\\repos\\RavenDbFinalTest\\RavenDbFinalTest\\certificate.pfx", "password");

            using (var store = new DocumentStore
            {
                Urls = new[] { " https://a.employeedb.ravendb.community" },
                Database = "TestEmployee",
                Certificate = certificate
            })
            {
                store.Initialize();


                using (var session = store.OpenSession())
                {
                   var emailExist = session.Query<Company>(collectionName:"Companies").Any(u=>u.EmailId== emailAddress);
                    ViewBag.EmailExists = emailExist;
                }

                return View();

            }
        }
 
 */