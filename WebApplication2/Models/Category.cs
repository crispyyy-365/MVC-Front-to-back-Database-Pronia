using System.ComponentModel.DataAnnotations;
using WebApplication2.Models.Base;

namespace WebApplication2.Models
{
	public class Category : BaseEntity
	{
		[Required]
		[MaxLength(30, ErrorMessage = "Max 30 characters !")]
		public string? Name { get; set; }

		//relational
		public List<Product>? products { get; set; }
	}
}