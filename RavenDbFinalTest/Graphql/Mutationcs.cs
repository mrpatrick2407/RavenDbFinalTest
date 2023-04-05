using GraphQL;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations.Attachments;
using RavenDbFinalTest.Models;
namespace RavenDbFinalTest.Graphql
{
    public class Mutationcs
    {
        private readonly IDocumentStore store;
        public Mutationcs(IDocumentStore documentStore)
        {
            store = documentStore;
        }

        public Productcs setproducts(string name,int pr)
        {
            using(var session=store.OpenSession())
                
            {
                var products = new Productcs { Price = pr ,Name=name};
                session.Store(products);
                session.SaveChanges();
                return products;
            }
        }

        [GraphQLName("savelogin")]

        public Login? savelogin([Service] IHttpContextAccessor accessor,string email)
        {
            using(var session=store.OpenSession())
            {
                var login = new Login
                {
                    EmailAddress = email,
                    IpAddress = accessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                    LoginTime = DateTime.UtcNow

                };
                session.Store(login, login.Id);
                session.SaveChanges();
                return login;
            }
        }

        public  string StoreImageAsync(string image,string userid)
        {
            using(var _session = store.OpenSession())
            {

                byte[] imageBytes = Convert.FromBase64String(image);
                using (MemoryStream stream = new MemoryStream(imageBytes))
                {
                    string contentType = "image/jpeg";
                     _session.Advanced.Attachments.Store(userid, "Profile.jpg", stream, "image/jpeg");
                     _session.SaveChanges();
                    return "Uploadded";
                }

            }
            
        }
        

        [GraphQLName("editdata")]
        public AdminRequest? editEmployee(Profile2 data)
        {

            using (var session = store.OpenSession())
            {
                var Employee = new AdminRequest
                {
                    eid = data.eid,
                    FirstName=data.FirstName,
                    LastName=data.LastName,
                    Department=data.Department,
                    Manager=data.Manager,
                    Job_Title=data.Job_Title,
                    Phone_Number=data.Phone_Number,
                    Email=data.Email,
                    Address=data.Address
                };
                session.Store(Employee);
                session.SaveChanges();
                return Employee;
            }

        }

    }
}
