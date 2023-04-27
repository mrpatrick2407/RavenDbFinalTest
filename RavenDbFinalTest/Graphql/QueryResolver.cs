using Raven.Client.Documents;
using RavenDbFinalTest.Models;
using Microsoft.AspNetCore.Mvc;
using RavenDbFinalTest.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Nest;
using Elastic.Clients.Elasticsearch;
using System.Security.Cryptography.X509Certificates;
namespace RavenDbFinalTest.Graphql
{
    public class QueryResolver
    {
        private readonly IDocumentStore _documentStore;
        private readonly HttpContextAccessor _contextAccessor;
        private readonly ElasticsearchClient _client;

        public QueryResolver(IDocumentStore documentStore, HttpContextAccessor httpContextAccessor, ElasticsearchClient client)
        {
            _documentStore = documentStore;
            _client = client;
            _contextAccessor = httpContextAccessor;
        }

        public bool EmailExists(string email)
        {
            using (var session = _documentStore.OpenSession())
            {
                Console.WriteLine(session.Query<Company>().Any(e => e.EmailId == email));
                return session.Query<Company>().Any(e => e.EmailId == email);
            }
        }

        [GraphQLName("pushtoelastic")]
        public async Task<bool> push()
        {
            List<Company> model = new List<Company>();

            using (var session = _documentStore.OpenSession())
            {

                model = session.Query<Company>(collectionName: "Companies").ToList();
            }
            var neww = await _client.Indices.DeleteAsync("company");

            var response = await _client.IndexManyAsync(model, "company");

            if (response.IsValidResponse)
            {
                Console.WriteLine($"Index document with ID {response.Items.First().Id} succeeded.");
                return true;
            }
            else
            {
                return false;
            }
        }

        [GraphQLName("getemployee")]
        public Company GetEmployee(string email)
        {
            using (var session = _documentStore.OpenSession())
            {

                var user = session.Query<Company>().FirstOrDefault(e => e.EmailId == email);
                if (user == null)
                {
                    return null;
                }
                else
                {
                    return user;
                }
            }
        }
        [GraphQLName("getimage")]
        public Profile2 getimage(int id)
        {
            using (var session = _documentStore.OpenSession())
            {
                var user = session.Query<Profile2>(collectionName: "Profile2").FirstOrDefault(e => e.eid == id);

                if (user != null)
                {
                    var attachment = session.Advanced.Attachments.Get(user.Id, "Profile.jpg");
                    if (attachment != null)
                    {
                        var imageStream = new MemoryStream();
                        attachment.Stream.CopyTo(imageStream);
                        user.ImageBase64 = Convert.ToBase64String(imageStream.ToArray());
                    }
                }
                return user;
            }
        }





        [GraphQLName("getemployeebyid")]
        public Profile2 GetEmployeebyid(int id)
        {

            using (var session = _documentStore.OpenSession())
            {

                var user = session.Query<Profile2>(collectionName: "Profile2").FirstOrDefault(e => e.eid == id);
                return user;
            }

        }


        [GraphQLName("getNotificationsByIdAndTimestamp")]
        public List<Notification> GetNotificationsByIdAndTimestamp(int id, DateTime timestamp)
        {
            using (var session = _documentStore.OpenSession())
            {
                var notifications = session.Query<Notification>(collectionName: "Notifications")
                .Where(e => e.Eid == id && e.Timestamp <= timestamp)
                .ToList();
                return notifications;
            }
        }
        [GraphQLName("newnotification")]
        public List<Notification> NewNotification(int id, DateTime timestamp)
        {
            using (var session = _documentStore.OpenSession())
            {
                var notifications = session.Query<Notification>(collectionName: "Notifications")
                .Where(e => e.Eid == id && e.Timestamp >= timestamp)
                .ToList();
                return notifications;
            }
        }


        [GraphQLName("getemployeereq")]
        public string getemployeereq(int id, bool val)
        {

            using (var session = _documentStore.OpenSession())
            {

                var tempuser = session.Query<Profile2>(collectionName: "Profile2").FirstOrDefault(e => e.eid == id);

                var req = session.Query<Profile2>(collectionName: "AdminRequests").FirstOrDefault(e => e.eid == id);

                string userid = session.Advanced.GetDocumentId(tempuser);
                var user = session.Load<Profile2>(userid);
                var companyuser = session.Query<Company>(collectionName: "Companies").FirstOrDefault(e => e.eid == id);

                if (val)
                {
                    if (user != null && req != null && companyuser != null)
                    {


                        session.Advanced.Patch(user, x => x.FirstName, req.FirstName);
                        session.Advanced.Patch(user, x => x.LastName, req.LastName);
                        session.Advanced.Patch(user, x => x.Email, req.Email);
                        session.Advanced.Patch(user, x => x.Phone_Number, req.Phone_Number);
                        session.Advanced.Patch(user, x => x.Address, req.Address);
                        session.Advanced.Patch(user, x => x.Job_Title, req.Job_Title);
                        session.Advanced.Patch(user, x => x.Department, req.Department);
                        session.Advanced.Patch(user, x => x.Manager, req.Manager);
                        companyuser.Name = req.FirstName + "" + req.LastName;
                        companyuser.Phone = req.Phone_Number;
                        companyuser.EmailId = req.Email;
                        companyuser.role = req.Department;


                        //storein notificationo collection
                        var notification = new Notification
                        {
                            Eid = req.eid,
                            name = new RNames
                            {
                                FirstName = req.FirstName,
                                LastName = req.LastName
                            },
                            Email = req.Email,
                            Message = "Your data is updated",
                            Status = true,
                            Timestamp = DateTime.UtcNow

                        };


                        session.Store(notification);
                        session.Delete(req);
                        session.SaveChanges();
                        return "approved";
                    }
                    else
                    {
                        return "appovedfailed";
                    }
                }
                else
                {
                    var req1 = session.Query<Profile2>(collectionName: "AdminRequests").FirstOrDefault(e => e.eid == id);

                    if (req1 != null)
                    {
                        var notification = new Notification
                        {
                            Eid = req.eid,
                            name = new RNames
                            {
                                FirstName = req.FirstName,
                                LastName = req.LastName
                            },
                            Email = req.Email,
                            Message = "Your request has been denied",
                            Status = false,

                        };
                        session.Store(notification);
                        session.Delete(req);
                        session.SaveChanges();
                        return "denied";
                    }
                    else
                    {
                        return "deniedfailed";
                    }
                }
            }
        }




        [GraphQLName("getadminreq")]
        public bool getadminreq(int id)
        {
            using (var session = _documentStore.OpenSession())
            {
                var user = session.Query<AdminRequest>(collectionName: "AdminRequests").FirstOrDefault(e => e.eid == id);
                if (user != null)
                {
                    return true;
                }
                return false;
            }
        }




        [GraphQLName("logactivity")]
        public string log(string email)
        {

            using (var session = _documentStore.OpenSession())
            {


                var user = session.Query<Company>().FirstOrDefault(e => e.EmailId == email);
                if (user == null)
                {
                    return "error";
                }
                else
                {
                    try
                    {
                        var login = new Login()
                        {
                            EmailAddress = email,
                            LoginTime = DateTime.UtcNow,


                        };
                        session.Store(login, login.Id);
                        session.SaveChanges();
                        return "savedchanges";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return "errorfromcacth";
                    }
                }

            }


        }




        //AdminController Queries


        [GraphQLName("getemployeebyidadmin")]
        public Company GetEmployeeById(string Id)
        {

            using (var session = _documentStore.OpenSession())
            {

                var user = session.Query<Company>().FirstOrDefault(e => e.Id == Id);
                if (user == null)
                {
                    return null;
                }
                else
                {
                    return user;
                }
            }
        }

        [GraphQLName("allemployeereq")]
        public IEnumerable<AdminRequest> employeereq()
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<AdminRequest>(collectionName: "AdminRequests").ToList();
            }
        }

        public IEnumerable<Company> GetAllEmployees()
        {

            using (var session = _documentStore.OpenSession())
            {
                return session.Query<Company>().ToList();
            }

        }
        [GraphQLName("birthdaynotification")]
        public EmployeeData? BirthdayNotification(string eid)
        {
            DateTime currentDate = DateTime.UtcNow.Date;
            string currentDateAsString = currentDate.ToString("yyyy-MM-dd");
            try
            {
                using (var session = _documentStore.OpenSession())
                {
                    var employee = session.Query<EmployeeData>(collectionName: "Employee")
                        .FirstOrDefault(e => e.EmpId == eid);
                    if (employee != null && employee.DateOfBirth.Substring(5, 5) == currentDateAsString.Substring(5, 5))
                    {
                        return employee;
                    }
                    else
                    {
                        return new EmployeeData { DateOfBirth = "Invalid" };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // return a default value or re-throw the exception
                return null;

            }
        }





    }

}