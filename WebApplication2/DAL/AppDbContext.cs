using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.DAL
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
		public DbSet<Slide> Slides { get; set; }
		public DbSet<Category> categories { get; set; }
		public DbSet<Product> products { get; set; }
		public DbSet<ProductImage> productImages { get; set; }
	}
}