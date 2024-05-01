using Motel.Utility.Database;
using Motel.Utility.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("BaseProjectDatabase")
);
//var client = new MongoClient("mongodb://localhost:27017");
//var database = client.GetDatabase("Motel");
//builder.Services.AddSingleton<IMongoCollection<UserAccount>>(database.GetCollection<UserAccount>("user_accounts"));

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

app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();