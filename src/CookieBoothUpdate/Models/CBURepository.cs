using System;
using System.Collections.Generic;
using System.Linq;

namespace CookieBoothUpdate.Models
{
    public class CBURepository : ICBURepository
    {
        readonly List<CBUItem> _items = new List<CBUItem>();

        public IEnumerable<CBUItem> AllItems
        {
            get
            {
                return _items;
            }
        }

        public CBUItem GetByZipcode(string zipcode)
        {
            return _items.FirstOrDefault(x => x.Zipcode == zipcode);
        }

        public void Add(CBUItem item)
        {
            item.Zipcode = 1 + _items.Max(x => x.Zipcode) ?? "";
            _items.Add(item);
        }

        public bool TryDelete(string zipcode)
        {
            var item = GetByZipcode(zipcode);
            if (item == null)
            {
                return false;
            }
            _items.Remove(item);
            return true;
        }
    }
} 