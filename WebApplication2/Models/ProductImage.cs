using Pronia.Models.Base;

namespace Pronia.Models
{
	public class ProductImage : BaseEntity
	{
		public string Image { get; set; }
		public bool? IsPrimary { get; set; }

		//relational
		public string ProductId { get; set; }
		public Product Product { get; set; }
	}
}