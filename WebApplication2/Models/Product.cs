using WebApplication2.Models.Base;

namespace WebApplication2.Models
{
	public class Product : BaseEntity
	{
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string Description { get; set; }
		public string SKU { get; set; }

		//relational
		public string CategoryName { get; set; }
		public Category Category { get; set; }
		public List<ProductImage> ProductImages { get; set; }
	}
}