using HealthCheck;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks()
    .AddCheck("ICMP_01",
        new ICMPHealthCheck("www.rydael.com", 100))
    .AddCheck("ICMP_02",
        new ICMPHealthCheck("www.google.com", 100))
    .AddCheck("ICMP_03",
        new ICMPHealthCheck("www.does-not-exist.com", 100))
    .AddCheck("ICMP_04",
        new ICMPHealthCheck("www.testmonkey.com", 100))
    .AddCheck("ICMP_05",
        new ICMPHealthCheck("heimdalsecurity.com", 100));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = context =>
    {
        // Retrieve cache configuration from appsettings.json
        context.Context.Response.Headers["Cache-Control"] = app.Configuration["StaticFiles:Headers:Cache-Control"];
    }
});
app.UseRouting();

app.MapHealthChecks("/hc", new CustomHealthCheckOptions());
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
