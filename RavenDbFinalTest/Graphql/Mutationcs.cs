using GraphQL;
using Raven.Client.Documents;
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

        public Login? savelogin(string email,string ip)
        {
            using(var session=store.OpenSession())
            {
                var login = new Login
                {
                    EmailAddress = email,
                    IpAddress = ip,
                    LoginTime = DateTime.UtcNow

                };
                session.Store(login, login.Id);
                session.SaveChanges();
                return login;
            }
        }

    }
}
