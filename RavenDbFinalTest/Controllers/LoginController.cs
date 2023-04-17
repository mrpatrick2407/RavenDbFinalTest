using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using Raven.Client.Documents;
using RavenDbFinalTest.Models;
using System.Security.Cryptography.X509Certificates;

namespace RavenDbFinalTest.Controllers
{
    public class LoginController : Controller
    {
        public async Task<IActionResult>  Index()
        {
            var store = new DocumentStore
            {
                Urls = new[] { "https://a.free.rmanojcei.ravendb.cloud" },
                Database = "TestEmployee",
                Certificate = new X509Certificate2("Cloud.pfx", "93EE9D996433A0E1B61FF03749B2AFC7")
            };
            store.Initialize();
            {
                using (var session = store.OpenSession())
                {
                    var logins= session.Query<Login>(collectionName:"Logins").ToList();
                        /*foreach (var login in logins)
                    {
                            Console.WriteLine($"EmailAddress: {login.EmailAddress}");
                            Console.WriteLine($"LoginTime: {login.LoginTime}");
                           
                    }*/
                    ViewBag.Login = logins;

                }
            }
            return View();
        }
       
    }
}
