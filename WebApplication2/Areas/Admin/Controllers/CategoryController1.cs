using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication2.DAL;
using WebApplication2.Models;

namespace WebApplication2.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CategoryController1 : Controller
	{
		public readonly AppDbContext _context;

		public CategoryController1(AppDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			List<Category> categories = await _context.Categories.Include(c => c.products).ToListAsync();

			return View(categories);
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(Category category)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			bool result = await _context.Categories.AnyAsync(c => c.Name.Trim() == category.Name.Trim());

			if (result)
			{
				ModelState.AddModelError("Name", "Category already exists");
				return View();
			}

		    category.CreatedAt = DateTime.Now;
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
	}
}
