using Pronia.Models;
using Pronia.Models.Base;

namespace Pronia.Models
{
	public class ProductTag : BaseEntity
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public int TagId { get; set; }
		public Product Product { get; set; }
		public Tag Tag { get; set; }
	}
}
