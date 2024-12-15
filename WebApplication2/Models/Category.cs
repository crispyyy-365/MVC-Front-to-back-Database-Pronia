using System.ComponentModel.DataAnnotations;
using Pronia.Models;
using Pronia.Models.Base;

namespace Pronia.Models
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