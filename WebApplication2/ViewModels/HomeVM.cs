using Pronia.Models;
using Pronia.Models;

namespace Pronia.ViewModels
{
	public class HomeVM
	{
		public List<Slide> Slides { get; set; }
		public List<Product> Products { get; set; }
		public List<ProductImage> Images { get; set; }
		public List<Category> Categories { get; set; }
	}
}
