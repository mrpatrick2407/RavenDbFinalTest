using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using RavenDbFinalTest.Models;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace RavenDbFinalTest.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public IActionResult Index()
        {
            List<Company> model = new List<Company>();
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
                    /*
                      session.Store(new Company
                      {
                         Name = "Vani HP",
                         ExternalId = "Trainee",
                        Phone = "+91 7856654357",
                       EmailId= "vanihp@ceiamerica.com",
                    });
                    */
                    session.SaveChanges();
                    model = session.Query<Company>(collectionName:"Companies").ToList();
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Company model)
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
                    session.Store(model);
                    session.SaveChanges();
                }
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult Details(string Id)
        {
            String FormatedId = Id.Replace("%2F", "/");
            Company model = new Company();
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
                    model = session.Load<Company>(FormatedId);
                }
            }
            return View(model);

        }
    }
}
