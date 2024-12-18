using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
	[Area("Admin")]
	//[Authorize(Roles = "Admin, Moderator")]
	[ValidateAntiForgeryToken]
	public class TagController : Controller
	{
		public AppDbContext _context { get; set; }
		public IWebHostEnvironment _env { get; set; }
		public TagController(AppDbContext contex, IWebHostEnvironment env)
		{
			_context = contex;
			_env = env;
		}
		public async Task<IActionResult> Index()
		{
			List<Tag> tags = await _context.Tags
				.Where(t => !t.IsDeleted)
				.Include(t => t.productTags)
				.ToListAsync();
			return View();
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(Tag tag)
		{
			if (!ModelState.IsValid) return View();
			bool result = await _context.Tags.AnyAsync(t => t.Name.Trim() == t.Name.Trim());
			if (result)
			{
				ModelState.AddModelError("Name", "Tag already exists");
				return View();
			}
			tag.CreatedAt = DateTime.Now;
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(int? id)
		{
			if (id == null || id < 1) return BadRequest();
			Tag? existed = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
			if (existed is null) return NotFound();
			return View(existed);
		}
		[HttpPost]
		public async Task<IActionResult> Update(int? id, Tag tag)
		{
			if (id == null || id < 1) return BadRequest();
			Tag? existed = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
			if (existed is null) return NotFound();
			if (!ModelState.IsValid) return View();
			bool result = await _context.Tags.AnyAsync(t => t.Name.Trim() == tag.Name.Trim() && t.Id != id);
			if (result)
			{
				ModelState.AddModelError(nameof(Tag.Name), "Tag already exixts");
				return View();
			}
			existed.Name = tag.Name;
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || id < 1) return BadRequest();
			Tag? tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
			if (tag is null) return NotFound();
			tag.IsDeleted = true;
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
	}
}
