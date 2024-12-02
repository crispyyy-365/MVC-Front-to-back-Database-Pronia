using Microsoft.AspNetCore.Mvc;

namespace Pronia.Areas.Admin.Controllers
{
	public class TagController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
