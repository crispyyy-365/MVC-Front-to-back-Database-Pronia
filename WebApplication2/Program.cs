using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pronia.Models;
using Pronia.DAL;
using Microsoft.Build.Framework;
using Pronia.Services.Interfaces;
using Pronia.Services.Implementations;

namespace Pronia
{
	public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
			{
				opt.Password.RequiredLength = 8;
				opt.Password.RequireNonAlphanumeric = false;
				opt.User.RequireUniqueEmail = true;
				opt.Lockout.AllowedForNewUsers = true;
				opt.Lockout.MaxFailedAccessAttempts = 3;
				opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
			}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

			builder.Services.AddControllersWithViews();

			builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

			builder.Services.AddScoped<ILayoutService, LayoutService>();
			var app = builder.Build();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseStaticFiles();

            app.MapControllerRoute(
                name: "admin",
                pattern: "{area:exists}/{controller=home}/{action=index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=home}/{action=index}/{id?}");

            app.Run();
        }
    }
}