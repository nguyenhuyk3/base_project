using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Motel.Middleware;
using Motel.Utility.Database;
using Motel.Utility.Hubs;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    options =>
    {
        options.LoginPath = "/Post";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy =>
      policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("RequireCustomer", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Customer")
    );
});

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("BaseProjectDatabase")
);

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


// Performs the mapping of a hub to a specific endpoint in your application.
/* 
    It does the following:
        1. 'app.MapHub<ChatHub>': Register a hub in your application. 
            In this case, 'ChatHub' is the name of the registered hub.
        2. '("/chatHub")': Map that hub to a specific endpoint. 
            In this case, the hub is mapped to the "/chatHub" endpoint. 
            This means that when the client wants to connect to the hub, 
            it sends a request to the application's "/chatHub" address.
*/
app.MapHub<ChatHub>("/chatHub");

//app.MapAreaControllerRoute(
//    name: "MyAreaProducts",
//    areaName: "Post",
//    pattern: "Post/{controller=Home}/{action=Index}/{id?}");

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapAreaControllerRoute(
//       name: "areaRoute",
//       areaName: "Post",
//       pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id?}");
//});

app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Post", controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();