using HotChocolate.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NuGet.Protocol.Plugins;
using Raven.Client.Documents;
using RavenDbFinalTest.Models;
using System.Security.Cryptography.X509Certificates;

namespace RavenDbFinalTest.Controllers
{
    public class LoginController : Controller
    {   

        private readonly IMemoryCache cache;
        public LoginController(IMemoryCache _cache) {
            cache = _cache;
        }
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
        public void Cookie( )
        {
            cache.Set("Loosu", "Moo", DateTime.Now.AddMinutes(10));
        }
        public IActionResult GetValue()
        {
            string value;

            // Try to get the value from the cache
           value= cache.Get("Loosu").ToString();
            
            return Content(value);
        }

    }
}
