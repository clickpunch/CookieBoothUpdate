using System.ComponentModel.DataAnnotations;

namespace CookieBoothUpdate.Models
{
	public class CBUItem
	{
		[Required]
		public string Zipcode { get; set; }
	}
	
}