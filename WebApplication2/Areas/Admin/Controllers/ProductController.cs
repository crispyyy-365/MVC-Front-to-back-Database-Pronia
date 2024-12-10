using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Admin.ViewModels;
using Pronia.Models;
using WebApplication2.DAL;
using WebApplication2.Models;
using WebApplication2.Utilities.Enums;
using WebApplication2.Utilities.Extensions;

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
			return View(productsVM);
		}
		[HttpPost]
		public async Task<IActionResult> Create(CreateProductVM productVM)
		{
			productVM.Categories = await _context.Categories.ToListAsync();
			productVM.Tags = await _context.Tags.ToListAsync();
			if(!ModelState.IsValid) return View(productVM);

			if (!productVM.MainPhoto.ValidateType("image/")) 
			{
				ModelState.AddModelError("MainPhoto", "File type is incorrect");
				return View(productVM);
			}
			if (!productVM.MainPhoto.ValidateSize(FileSize.MB, 1))
			{
				ModelState.AddModelError("MainPhoto", "File size is incorrect");
				return View(productVM);
			}
			if (!productVM.HoverPhoto.ValidateType("image/"))
			{
				ModelState.AddModelError("HoverPhoto", "File type is incorrect");
				return View(productVM);
			}
			if (!productVM.HoverPhoto.ValidateSize(FileSize.MB, 1))
			{
				ModelState.AddModelError("HoverPhoto", "File size is incorrect");
				return View(productVM);
			}
			bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
			if (!result)
			{
				ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "Category does not exist !");
				return View(productVM);
			}
			if (productVM.TagIds is not null)
			{
				bool tagResult = productVM.TagIds.Any(tId => !productVM.Tags.Exists(t => t.Id == tId));
				if (tagResult)
				{
					ModelState.AddModelError(nameof(CreateProductVM.TagIds), "Tags are wrong");
					return View(productVM);
				}
			}
			ProductImage main = new()
			{
				Image = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
				IsPrimary = true,
				CreatedAt = DateTime.Now,
				IsDeleted = false,
			};
			ProductImage hover = new()
			{
				Image = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
				IsPrimary = true,
				CreatedAt = DateTime.Now,
				IsDeleted = false,
			};
			Product product = new()
			{
				Name = productVM.Name,
				SKU = productVM.SKU,
				CategoryId = productVM.CategoryId.Value,
				Description = productVM.Description,
				Price = productVM.Price.Value,
				CreatedAt = DateTime.Now,
				IsDeleted = false,
				ProductImages = new List<ProductImage> { main, hover }
			};
			if(productVM.TagIds is not null)
			{
				product.ProductTags = productVM.TagIds.Select(tId => new ProductTag { TagId = tId }).ToList();
			}
			if(productVM.AdditionalPhotos is not null)
			{
				string text = string.Empty;
				foreach (IFormFile file in productVM.AdditionalPhotos)
				{
					if (!file.ValidateType("/image"))
					{
						text += $"<p class=\"text-danger\">{file.FileName} invalid type</p>";
						continue;
					}
					if (!file.ValidateSize(FileSize.MB, 1))
					{
						text += $"<p class=\"text-danger\">{file.FileName} invalid size</p>";
						continue;
					}
					product.ProductImages.Add(new ProductImage
					{
						Image = await file.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
						CreatedAt = DateTime.Now,
						IsDeleted = false,
						IsPrimary = null,
					});
				}
				TempData["FileWarning"] = text;
			}
			await _context.Products.AddAsync(product);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Update(int? id)
		{
			if (id is null || id < 0) return BadRequest();
			Product? product = await _context.Products.Include(p => p.ProductImages).Include(p => p.ProductTags).FirstOrDefaultAsync(c => c.Id == id);
			if (product is null) return NotFound();
			UpdateProductVM productVM = new()
			{
				Name = product.Name,
				SKU = product.SKU,
				CategoryId = product.CategoryId,
				Description = product.Description,
				Price = product.Price,
				ProductImages = product.ProductImages,
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
			Product? existed = await _context.Products.Include(p => p.ProductImages).Include(p => p.ProductTags).FirstOrDefaultAsync(c => c.Id == id);
			if (existed is null) return NotFound();
			productVM.Categories = await _context.Categories.ToListAsync();
			productVM.Tags = await _context.Tags.ToListAsync();
			productVM.ProductImages = existed.ProductImages;
			if(!ModelState.IsValid) return View(productVM);

			if (productVM.ProductImages is not null) 
			{
				if (!productVM.MainPhoto.ValidateType("image/"))
				{
					ModelState.AddModelError("MainPhoto", "File type is incorrect");
					return View(productVM);
				}
				if (!productVM.MainPhoto.ValidateSize(FileSize.MB, 1))
				{
					ModelState.AddModelError("MainPhoto", "File size is incorrect");
					return View(productVM);
				}
			}
			if (productVM.HoverPhoto is not null)
			{
				if (!productVM.MainPhoto.ValidateType("image/"))
				{
					ModelState.AddModelError("HoverPhoto", "File type is incorrect");
					return View(productVM);
				}
				if (!productVM.MainPhoto.ValidateSize(FileSize.MB, 1))
				{
					ModelState.AddModelError("HoverPhoto", "File size is incorrect");
					return View(productVM);
				}
			}
			if (existed.CategoryId != productVM.CategoryId)
			{
				bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
				if (!result) return View(productVM);
			}
			if (productVM.TagIds is not null)
			{
				bool tagResult = productVM.TagIds.Any(tId => !productVM.Tags.Exists(t => t.Id == tId));
				if (tagResult)
				{
					ModelState.AddModelError(nameof(CreateProductVM.TagIds), "Tags are wrong");
					return View(productVM);
				}
			}
			if (productVM.TagIds is not null)
			{
				productVM.TagIds = new();
			}
			else
			{
				productVM.TagIds = productVM.TagIds.Distinct().ToList();
			}

			_context.ProductTags.RemoveRange(existed.ProductTags
				.Where(pTag => !productVM.TagIds.Exists(tId => tId == pTag.TagId))
				.ToList());
			_context.ProductTags.AddRange(productVM.TagIds
				.Where(tId => !existed.ProductTags.Exists(pTag => pTag.TagId == tId))
				.ToList()
				.Select(tId => new ProductTag { TagId = tId, ProductId = existed.Id }));

			if (productVM.MainPhoto is not null)
			{
				string fileName = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
				ProductImage main = existed.ProductImages.FirstOrDefault(p => p.IsPrimary == true);
				main.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

				existed.ProductImages.Remove(main);
				existed.ProductImages.Add(new ProductImage
				{
					CreatedAt = DateTime.Now,
					IsDeleted = false,
					IsPrimary = true,
					Image = fileName,
				});
			}
			if (productVM.HoverPhoto is not null)
			{
				string fileName = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
				ProductImage hover = existed.ProductImages.FirstOrDefault(p => p.IsPrimary == false);
				hover.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

				existed.ProductImages.Remove(hover);
				existed.ProductImages.Add(new ProductImage
				{
					CreatedAt = DateTime.Now,
					IsDeleted = false,
					IsPrimary = false,
					Image = fileName,
				});
			}
			if (productVM.ImageIds is null)
			{
				productVM.ImageIds = new List<int>();
			}
			var deletedImages = existed.ProductImages
				.Where(pi => !productVM.ImageIds
				.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == null)
				.ToList();
			deletedImages
				.ForEach(di => di.Image
				.DeleteFile(_env.WebRootPath, "assets", "images", "website-images"));
			_context.ProductImages.RemoveRange(deletedImages);

			if (productVM.AdditionalPhotos is not null)
			{
				string text = string.Empty;
				foreach (IFormFile file in productVM.AdditionalPhotos)
				{
					if (!file.ValidateType("/image"))
					{
						text += $"<p class=\"text-danger\">{file.FileName} invalid type</p>";
						continue;
					}
					if (!file.ValidateSize(FileSize.MB, 1))
					{
						text += $"<p class=\"text-danger\">{file.FileName} invalid size</p>";
						continue;
					}
					existed.ProductImages.Add(new ProductImage
					{
						Image = await file.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
						CreatedAt = DateTime.Now,
						IsDeleted = false,
						IsPrimary = null,
					});
				}
				TempData["FileWarning"] = text;
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