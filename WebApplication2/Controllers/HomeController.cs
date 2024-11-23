﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
			HomeVM homeVm = new HomeVM
			{
				Slides = _context.Slides
				.OrderBy(s => s.Order)
				.ToList(),

				Products = _context.Products
				.Where(p => p.IsDeleted != false)
				.Take(8)
				.Include(p => p.ProductImages.Where(p => p.IsPrimary != false))
				.ToList()
			};

		    //_context.SaveChanges();

			return View(homeVm);
		}
	}
}