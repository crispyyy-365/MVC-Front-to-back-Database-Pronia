﻿using WebApplication2.Models.Base;

namespace WebApplication2.Models
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
