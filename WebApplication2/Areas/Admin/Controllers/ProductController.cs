using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Pronia.Areas.Admin.ViewModels;
using WebApplication2.DAL;

namespace Pronia.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		public AppDbContext _context { get; set; }
		public IWebHostEnvironment _env { get; set; }
		public ProductController(AppDbContext dbContext, IWebHostEnvironment env)
		{
			_context = dbContext;
			_env = env;
		}
		public async Task<IActionResult> Index()
		{
			List<GetProductAdminVM> productsVMs= await _context.Products
				.Include(p => p.Category)
				.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
				.Select(p=>new GetProductAdminVM
				{
					Name = p.Name,
					Price = p.Price,
					CategoryName = p.CategoryName,
					Image = p.ProductImages[0].Image,
				})
				.ToListAsync();

			
			return View(productsVMs);
		}
	}
}
