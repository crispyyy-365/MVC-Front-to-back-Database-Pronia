using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.DAL
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
		
		public DbSet<Slide> Slides { get; set; }
	}
}