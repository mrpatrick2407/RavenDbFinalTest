using Raven.Client.Documents;
using RavenDbFinalTest.Models;
namespace RavenDbFinalTest.Graphql
{
    public class QueryResolver
    {
        private readonly IDocumentStore _documentStore;

        public QueryResolver(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public bool EmailExists(string email)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<Company>().Any(e => e.EmailId == email);
            }
        }
    }

}
