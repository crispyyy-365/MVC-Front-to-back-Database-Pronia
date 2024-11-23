using WebApplication2.Models;

namespace WebApplication2.ViewModels
{
	public class HomeVM
	{
		public List<Slide> Slides { get; set; }
		public List<Product> Products { get; set; }
		public List<ProductImage> Images { get; set; }
		public List<Category> Categories { get; set; }
	}
}