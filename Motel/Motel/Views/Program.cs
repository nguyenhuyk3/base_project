using MongoDB.Driver;
using WebProject.Models;
using WebProject.Utility.Database;
using WebProject.Utility.Hubs;

namespace WebProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();

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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
