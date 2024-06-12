using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Motel.Middleware;
using Motel.Utility.Address;
using Motel.Utility.Database;
using Motel.Utility.Hubs;
using System.Configuration;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
// Read configuration from appsettings.json
var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    options =>
    {
        options.LoginPath = "/Post";
    });

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        // Đọc thông tin Authentication:Google từ appsettings.json
        IConfigurationSection googleAuthNSection = configuration.GetSection("Authentication:Google");
        // Thiết lập ClientID và ClientSecret để truy cập API google
        googleOptions.ClientId = googleAuthNSection["ClientId"];
        googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
        // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
        googleOptions.CallbackPath = "/LoginWithGoogle";
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

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".WebBooking.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

app.UseSession();

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