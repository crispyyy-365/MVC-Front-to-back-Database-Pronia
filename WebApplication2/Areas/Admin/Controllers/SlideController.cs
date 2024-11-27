using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using WebApplication2.DAL;
using WebApplication2.Models;
using WebApplication2.Utilities.Extensions;

namespace WebApplication2.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class SlideController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;

		public SlideController(AppDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}
		public async Task<IActionResult> Index()
		{
			List<Slide> slides = await _context.Slides.ToListAsync();

			return View(slides);
		}
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Slide slide)
		{
			if (!slide.Photo.ValidateType("image")) 
			{
				ModelState.AddModelError("Photo", "File type is incorrect");
				
				return View();
			}
			if (slide.Photo.ValidateSize(Utilities.Enums.FileSize.MB, 2)) 
			{
				ModelState.AddModelError("Photo", "File size must be less than 2mb");
				
				return View();
			}

			slide.Image = await slide.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");

			await _context.Slides.AddAsync(slide);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || id < 1) return BadRequest();

			Slide? slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

			if (slide == null) return NotFound();

			slide.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

			_context.Slides.Remove(slide);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
	}
}