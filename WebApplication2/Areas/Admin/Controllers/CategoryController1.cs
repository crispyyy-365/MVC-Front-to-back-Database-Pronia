using Azure.Core;
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
			List<Category> categories = await _context.Categories.Where(c => !c.IsDeleted).Include(c => c.products).ToListAsync();

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

		public async Task<IActionResult> Update(int?id)
		{
			if (id == null || id < 1) return BadRequest();
			
			Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

			if (category is null) return NotFound();

			return View(category);
		}

		[HttpPost]
		public async Task<IActionResult> Update(int? id, Category category)
		{
			if (id == null || id < 1) return BadRequest();

			Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

			if (category is null) return NotFound();

			if (!ModelState.IsValid)
			{
				return View();
			}

			bool result = await _context.Categories.AnyAsync(c => c.Name.Trim() == category.Name.Trim() && c.Id != id);

			if (result)
			{
				ModelState.AddModelError(nameof(Category.Name), "Category already exixts");
				return View();
			}

			existed.Name = category.Name;
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));	

		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || id < 1) return BadRequest();

			Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

			if (category is null) return NotFound();

			
			category.IsDeleted = true;

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
	}
}
