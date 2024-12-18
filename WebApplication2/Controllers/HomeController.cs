using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
	public class HomeController : Controller
	{
		public readonly AppDbContext _context;

		public HomeController(AppDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			HomeVM homeVm = new HomeVM
			{
				Slides = await _context.Slides
				.OrderBy(s => s.Order)
				.ToListAsync(),

				Products = await _context.Products
				.Where(p => p.IsDeleted != false)
				.Take(8)
				.Include(p => p.ProductImages.Where(p => p.IsPrimary != false))
				.ToListAsync(),

				NewProducts = await _context.Products
				.OrderByDescending(p => p.CreatedAt)
				.Where(p => p.IsDeleted != false)
				.Take(8)
				.Include(p => p.ProductImages.Where(p => p.IsPrimary != false))
				.ToListAsync()
			};

			//_context.SaveChanges();

			return View(homeVm);
		}
	}
}