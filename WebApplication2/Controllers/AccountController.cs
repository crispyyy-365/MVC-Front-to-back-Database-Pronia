using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Models;
using Pronia.ViewModels;
using Microsoft.EntityFrameworkCore;
using Pronia.Utilities.Enums;

namespace Pronia.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AccountController(UserManager<AppUser> userManager,
			SignInManager<AppUser> signInManager,
			RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterVM userVM)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			AppUser user = new AppUser
			{
				Name = userVM.Name,
				Surname = userVM.Surname,
				Email = userVM.Email,
				UserName = userVM.UserName
			};
			IdentityResult result = await _userManager.CreateAsync(user, userVM.Password);
			if (!result.Succeeded)
			{
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return View();
			}
			await _userManager.AddToRoleAsync(user, UserRole.Member.ToString());
			await _signInManager.SignInAsync(user, false);
			return RedirectToAction(nameof(HomeController.Index), "Home");
		}
		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction(nameof(HomeController.Index), "Home");
		}
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginVM userVM, string? returnUrl)
		{
			if(!ModelState.IsValid)
			{
				return View();
			}
			AppUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userVM.EmailOrUserName || u.Email == userVM.EmailOrUserName);
			if (user == null)
			{
				ModelState.AddModelError(string.Empty, "Email or Username or Password is incorrect");
				return View();
			}
			var result = await _signInManager.PasswordSignInAsync(user, userVM.Password, userVM.IsPersistent, true);
			if (!result.Succeeded)
			{
				ModelState.AddModelError(string.Empty, "Email or Username or Password is incorrect");
				return View();
			}
			if (result.IsLockedOut)
			{
				ModelState.AddModelError(string.Empty, "Your account is locked, try again later");
				return View();
			}
			if (returnUrl is null)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			} 
			return Redirect(returnUrl);
		}
		public async Task<IActionResult> CreateRole()
		{
			foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
			{
				await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
			}
			return RedirectToAction(nameof(HomeController.Index), "Home");
		}
	}
}
