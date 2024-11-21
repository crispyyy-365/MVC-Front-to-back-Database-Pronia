using Microsoft.AspNetCore.Mvc;
using WebApplication2.DAL;
using WebApplication2.Models;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
	public class HomeController : Controller
	{
		public readonly AppDbContext _context;

		public HomeController(AppDbContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			List<Slide> slides = _context.Slides.OrderBy(s=>s.Order).ToList();

			HomeVM homeVm = new HomeVM
			{
				Slides = slides
			};
			return View(homeVm);
		}
	}
}