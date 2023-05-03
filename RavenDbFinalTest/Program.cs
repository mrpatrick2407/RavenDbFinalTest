using Raven.Client.Documents;
using System.Security.Cryptography.X509Certificates;
using HotChocolate.AspNetCore;
using RavenDbFinalTest.Graphql;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using System.Runtime.InteropServices;
using AspNetCore.Firebase.Authentication;
using FirebaseAdmin;
using RavenDbFinalTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Security.Claims;

var certificate = new X509Certificate2("Cloud.pfx", "93EE9D996433A0E1B61FF03749B2AFC7");



var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQL(sp => SchemaBuilder.New()
        .AddQueryType<QueryResolver>()
        .AddMutationType<Mutationcs>()
        .Create());
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);
});
builder.Services.AddSingleton<IDocumentStore>(provider =>
    new DocumentStore
    {
        Urls = new[] { "https://a.free.rmanojcei.ravendb.cloud" },
        Database = "TestEmployee",
        Certificate = new X509Certificate2("Cloud.pfx", "93EE9D996433A0E1B61FF03749B2AFC7")
    }.Initialize());
builder.Services.AddMemoryCache();

/*Elastic local
 var settings = new ElasticsearchClientSettings(new Uri("https://localhost:9200"))
                .CertificateFingerprint("3df6a5aad7a93475424b4d84e5707c258f2020457634503d56ab21e9bf7870dc")
                .Authentication(new BasicAuthentication("elastic", "DVi=1QeYRbmfcNMTpa58"));

 */
var settings = new ElasticsearchClientSettings(new Uri("https://my-deployment-bd707b.es.us-central1.gcp.cloud.es.io"))
                .Authentication(new BasicAuthentication("elastic", "FUTpHUkZKc467OjMr4AU1jba"));

var client = new ElasticsearchClient(settings);
builder.Services.AddSingleton<ElasticsearchClient>(client);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


/*builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
       
       
        
    };
});
*/
//    .AddSchemaFromFile("./GraphQL/schema.graphql"); 


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL("/graphql");


app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
