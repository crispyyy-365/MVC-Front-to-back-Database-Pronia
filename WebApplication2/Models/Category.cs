using WebApplication2.Models.Base;

namespace WebApplication2.Models
{
	public class Category : BaseEntity
	{
		public string Name { get; set; }
		public List<Product> products { get; set; }
	}
}
