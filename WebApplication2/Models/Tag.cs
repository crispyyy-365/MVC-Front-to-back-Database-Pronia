using WebApplication2.Models.Base;

namespace Pronia.Models
{
	public class Tag : BaseEntity
	{
		public string Name { get; set; }
		//relational
		public List<ProductTag> productTags { get; set; }
	}
}
