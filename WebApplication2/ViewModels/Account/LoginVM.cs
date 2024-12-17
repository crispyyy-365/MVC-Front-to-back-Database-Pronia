using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels
{
	public class LoginVM
	{
		[MinLength(4)]
		[MaxLength(255)]
		public string EmailOrUserName { get; set; }
		[MinLength(8)]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		public bool IsPersistent { get; set; }
	}
}
