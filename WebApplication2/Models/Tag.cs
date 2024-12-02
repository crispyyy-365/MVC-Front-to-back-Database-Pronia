using System.ComponentModel.DataAnnotations;
using WebApplication2.Models.Base;

namespace Pronia.Models
{
	public class Tag : BaseEntity
	{
		[Required]
		[MaxLength(30, ErrorMessage = "Max 30 characters !")]
		public string Name { get; set; }
		//relational
		public List<ProductTag> productTags { get; set; }
	}
}
