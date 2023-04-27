using GraphQL;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations.Attachments;
using RavenDbFinalTest.Models;
using System.Net.Http.Headers;

namespace RavenDbFinalTest.Graphql
{
    public class Mutationcs
    {
        private readonly IDocumentStore store;
        public Mutationcs(IDocumentStore documentStore)
        {
            store = documentStore;
        }



        [GraphQLName("CheakImage")]
        public async Task<string> CheckImageAsync(string image)
        {
            // Convert base64 image string to byte array
            byte[] imageBytes = Convert.FromBase64String(image);

            // Create HTTP client
            HttpClient httpClient = new HttpClient();

            // accesstoken

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "NERsMFFtdHhzbWU5bEhXRnJuYWpYeGNsOnFjTHdTSFo4WWwzQXo3dVdIOFpoM2tObjlpaXVpRWk2eDRhOTBQTmlsNlFpNEc2dw==");

            // Create multipart form data content
            MultipartFormDataContent content = new MultipartFormDataContent();
            ByteArrayContent imageContent = new ByteArrayContent(imageBytes);
            content.Add(imageContent, "data", "image.jpg");

            // Send POST request
            HttpResponseMessage response = await httpClient.PostAsync("https://api.everypixel.com/v1/quality_ugc", content);

            // Read response content
            string responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }



        public Product setproducts(string name, int pr)
        {
            try
            {
                using (var session = store.OpenSession())

                {
                    var productId = "products/" + Guid.NewGuid().ToString();
                    
                    var products = new Product { Id = productId, Price = pr, Name = name };
                    
                    session.Store(products);
                    session.SaveChanges();
                    return products;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return new Product { Price = 121 };
            }
        }

        [GraphQLName("savelogin")]

        public Login? savelogin([Service] IHttpContextAccessor accessor, string email)
        {
            using (var session = store.OpenSession())
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

        public string StoreImageAsync(string image, string userid)
        {
            using (var _session = store.OpenSession())
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
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Department = data.Department,
                    Manager = data.Manager,
                    Job_Title = data.Job_Title,
                    Phone_Number = data.Phone_Number,
                    Email = data.Email,
                    Address = data.Address
                };
                session.Store(Employee);
                session.SaveChanges();
                return Employee;
            }

        }


        //Admincontroller functions starts here
        public Company CreateEmployee(int eid, string Name, string role, string Phone, string EmailId)
        {
            using (var session = store.OpenSession())
            {
                var Employee = new Company()
                {

                    eid = eid,
                    Name = Name,
                    role = role,

                    Phone = Phone,
                    EmailId = EmailId
                };
                session.Store(Employee);
                session.SaveChanges();
                return Employee;
            }
        }

        public Company EditEmployee(string id, int eid, string Name, string role, string Phone, string EmailId)
        {
            using (var session = store.OpenSession())
            {
                var Employee = new Company()
                {

                    eid = eid,
                    Name = Name,
                    role = role,
                    Id = id,
                    Phone = Phone,
                    EmailId = EmailId
                };
                session.Store(Employee);
                session.SaveChanges();
                return Employee;
            }
        }

        [GraphQLName("deletenotificationbyid")]
        public Notification DeleteNotificationById(string id)
        {

            using (var session = store.OpenSession())
            {



                var notification = session.Query<Notification>(collectionName: "Notifications").FirstOrDefault(e => e.Id == id);

                session.Delete(notification);
                session.SaveChanges();
                return notification;

            }


        }

        public Company DeleteEmployeeById(int EId)
        {

            using (var session = store.OpenSession())
            {

                var Employee = session.Query<Company>().FirstOrDefault(e => e.eid == EId);
                if (Employee == null)
                {
                    return null;
                }
                else
                {
                    session.Delete(Employee);
                    session.SaveChanges();
                    return Employee;
                }
            }



        }
    }
}