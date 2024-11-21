using Microsoft.EntityFrameworkCore;
using WebApplication2.DAL;

namespace WebApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer("server=LAPTOP-E5JPOQ6L\\SQLEXPRESS;database=Pronia;trusted_connection=true;integrated security=true;TrustServerCertificate=true;"));

            var app = builder.Build();

            app.UseStaticFiles();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=home}/{action=index}/{id?}");

            app.Run();
        }
    }
}