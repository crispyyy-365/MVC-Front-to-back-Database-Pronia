using WebApplication2.Models;

namespace WebApplication2.ViewModels
{
	public class DetailVM
	{
		public Product Product { get; set; }
		public List<Product> RelatedProducts { get; set; }
	}
}