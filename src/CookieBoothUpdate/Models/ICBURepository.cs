using System.Collections.Generic;

namespace CookieBoothUpdate.Models
{
	public interface ICBURepository
	{
		IEnumerable<CBUItem> AllItems { get; }
		void Add(CBUItem item);
		CBUItem GetByZipcode(string zipcode);
		bool TryDelete(string zipcode);
	}
}