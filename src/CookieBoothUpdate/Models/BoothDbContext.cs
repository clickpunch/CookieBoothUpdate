using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace CookieBoothUpdate.Models
{
    public class BoothDbContext : DbContext
    {
        public DbSet<Booth> Booths { get; set; }
    }
}
