using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Pronia.Areas.Admin.ViewModels;
using Pronia.Areas.Admin.ViewModels.Products;
using Pronia.Models;
using WebApplication2.DAL;
using WebApplication2.Models;

namespace Pronia.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		public AppDbContext _context { get; set; }
		public IWebHostEnvironment _env { get; set; }
		public ProductController(AppDbContext dbContext, IWebHostEnvironment env)
		{
			_context = dbContext;
			_env = env;
		}
		public async Task<IActionResult> Index()
		{
			List<GetProductAdminVM> productsVMs= await _context.Products
				.Include(p => p.Category)
				.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
				.Select(p=>new GetProductAdminVM
				{
					Id = p.Id,
					Name = p.Name,
					Price = p.Price,
					CategoryName = p.Category.Name,
					Image = p.ProductImages[0].Image,
				})
				.ToListAsync();
			return View(productsVMs);
		}
		public async Task<IActionResult> Create()
		{
			CreateProductVM productsVM = new CreateProductVM()
			{
				Tags = await _context.Tags.ToListAsync(),
				Categories = await _context.Categories.ToListAsync(),
			};
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(CreateProductVM productVM)
		{
			productVM.Categories = await _context.Categories.ToListAsync();
			productVM.Tags = await _context.Tags.ToListAsync();
			if(!ModelState.IsValid)
			{
				return View(productVM);
			}
			foreach(var TId in productVM.TagIds)
			{
				bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
				if (!result)
				{
					ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "Category does not exist !");
					return View(productVM);
				}
			}
			if(productVM.TagIds is not null)
			{
				bool tagResult = productVM.TagIds.Any(tId => !productVM.Tags.Exists(t => t.Id == tId));
				if (tagResult)
				{
					ModelState.AddModelError(nameof(CreateProductVM.TagIds), "Tags are wrong");
					return View(productVM);
				}
			}
			Product product = new()
			{
				Name = productVM.Name,
				SKU = productVM.SKU,
				CategoryId = productVM.CategoryId.Value,
				Description = productVM.Description,
				Price = productVM.Price.Value,
				CreatedAt = DateTime.Now,
				IsDeleted = false,
			};
			if(productVM.TagIds is not null)
			{
				product.ProductTags = productVM.TagIds.Select(tId => new ProductTag { TagId = tId }).ToList();
			}
			//foreach(int tId in productVM.TagIds)
			//{
			//	product.ProductTags.Add(new ProductTag
			//	{
			//		TagId = tId,
			//		Product = product,

			//	});
			//}
			await _context.Products.AddAsync(product);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Update(int? id)
		{
			if (id is null || id < 0) return BadRequest();
			Product product = await _context.Products.Include(p => p.ProductTags).FirstOrDefaultAsync(c => c.Id == id);
			if (product is null) return NotFound();
			UpdateProductVM productVM = new()
			{
				Name = product.Name,
				SKU = product.SKU,
				CategoryId = product.CategoryId,
				Description = product.Description,
				Price = product.Price,
				Categories = await _context.Categories.ToListAsync(),
				TagIds = product.ProductTags.Select(p => p.TagId).ToList(),
				Tags = await _context.Tags.ToListAsync()
			};
			return View(productVM);
		}
		[HttpPost]
		public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
		{
			if (id is null || id < 0) return BadRequest();
			productVM.Categories = await _context.Categories.ToListAsync();
			if(!ModelState.IsValid) return View(productVM);
			Product existed = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
			if (existed is null) return NotFound();
			if (existed.CategoryId != productVM.CategoryId) 
			{
				bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
				if (!result) return View(productVM);
			}
			existed.SKU = productVM.SKU;
			existed.Price = productVM.Price.Value;
			existed.CategoryId = productVM.CategoryId.Value;
			existed.Description = productVM.Description;
			existed.Name = productVM.Name;
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
	}
}