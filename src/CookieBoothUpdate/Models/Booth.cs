using System;
using System.ComponentModel.DataAnnotations;

namespace CookieBoothUpdate.Models
{
    public class Booth
    {
        [Required]
        public long ID { get; set; }
        public string CouncilID { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string TroopName { get; set; }
        [Required]
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        [Required]
        public string TimeOpen { get; set; }
        public string TimeClose { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ServiceUnit { get; set; }
        public string Active { get; set; }
        public string DateCreated { get; set; }
        public string Comment { get; set; }
        public long LBBID { get; set; }
    }
}
