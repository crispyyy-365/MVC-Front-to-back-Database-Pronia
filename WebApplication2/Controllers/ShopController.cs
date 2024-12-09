using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebApplication2.DAL;
using WebApplication2.Models;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
	public class ShopController : Controller
	{
		public readonly AppDbContext _context;
		public ShopController(AppDbContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> Detail(int? id)
		{
			if (id == null || id < 0) return BadRequest();

			Product? product = await _context.Products
				.Include(p => p.ProductImages.OrderByDescending(pi => pi.IsPrimary))
				.Include(p => p.Category)
				.Include(p => p.ProductTags)
				.ThenInclude(p => p.Tag)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (product is null) return NotFound();

			DetailVM detailVM = new DetailVM
			{
				Product = product,

				RelatedProducts = await _context.Products
				.Where(p => p.CategoryId == product.CategoryId && p.Id != id)
				.Include(p => p.ProductImages.Where(pi => pi != null))
				.Take(8)
				.ToListAsync(),
			};

			return View(detailVM);
		}
	}
}