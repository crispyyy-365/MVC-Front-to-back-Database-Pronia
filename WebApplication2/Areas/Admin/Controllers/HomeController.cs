using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles="Admin, Moderator")]
	[ValidateAntiForgeryToken]
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
