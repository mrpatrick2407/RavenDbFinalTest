using Raven.Client.Documents;
using System.Security.Cryptography.X509Certificates;
using HotChocolate.AspNetCore;
using RavenDbFinalTest.Graphql;


var certificate = new X509Certificate2("ClientCertificate.pfx", "Theophilus");


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQL(sp => SchemaBuilder.New()
        .AddQueryType<QueryResolver>()
        .Create());
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(500);
});
builder.Services.AddSingleton<IDocumentStore>(provider =>
    new DocumentStore
    {
        Urls = new[] { "https://a.theophilus.ravendb.community" },
        Database = "TestEmployee",
        Certificate = new X509Certificate2("ClientCertificate.pfx", "Theophilus")
    }.Initialize());
builder.Services.AddGraphQLServer()
        .AddQueryType<QueryResolver>();
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


app.MapGraphQL("/graphql");


app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
