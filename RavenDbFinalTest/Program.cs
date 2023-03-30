using Raven.Client.Documents;
using System.Security.Cryptography.X509Certificates;
using HotChocolate.AspNetCore;
using RavenDbFinalTest.Graphql;
using System.Runtime.InteropServices;

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
    options.IdleTimeout = TimeSpan.FromMinutes(500);
});
builder.Services.AddSingleton<IDocumentStore>(provider =>
    new DocumentStore
    {
        Urls = new[] { "https://a.free.rmanojcei.ravendb.cloud" },
        Database = "TestEmployee",
        Certificate = new X509Certificate2("Cloud.pfx", "93EE9D996433A0E1B61FF03749B2AFC7")
    }.Initialize());
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
