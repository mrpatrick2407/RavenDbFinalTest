using Raven.Client.Documents;
using RavenDbFinalTest.Models;
using Microsoft.AspNetCore.Mvc;
using RavenDbFinalTest.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RavenDbFinalTest.Graphql
{
    public class QueryResolver
    {
        private readonly IDocumentStore _documentStore;
        private readonly HttpContextAccessor _contextAccessor;

        public QueryResolver(IDocumentStore documentStore,HttpContextAccessor httpContextAccessor)
        {
            _documentStore = documentStore;
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
        [GraphQLName("getemployee")]
        public Company GetEmployee(string email)
        {            
            using (var session =_documentStore.OpenSession())
            {

                var user=session.Query<Company>().FirstOrDefault(e => e.EmailId == email);
                if(user== null)
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
                
                var user = session.Query<Profile2>(collectionName:"Profile2").FirstOrDefault(e => e.eid == id);
                return user;
            }

        }


        [GraphQLName("getadminreq")]
        public bool getadminreq(int id)
        {
            using(var session = _documentStore.OpenSession())
            {
                var user = session.Query<AdminRequest>(collectionName: "AdminRequests").FirstOrDefault(e => e.eid == id);
                if(user != null)
                {
                    return true;
                }
                return false;
            }
        }

       


            [GraphQLName("logactivity")]
            public string log(string email) {
            
                    using (var session= _documentStore.OpenSession())
                    {
                

                        var user = session.Query<Company>().FirstOrDefault(e=>e.EmailId==email);
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



        public IEnumerable<Company> GetAllEmployees()
        {

            using (var session = _documentStore.OpenSession())
            {
                return session.Query<Company>().ToList();
            }

        }



    }

}
