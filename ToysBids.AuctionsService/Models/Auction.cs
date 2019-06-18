
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ToysBids.AuctionsService.Models
{
    public class Publication
    {
        public long Id { get; set; }
        public long auctionBundleId { get; set; }
        [Column(name: "StartAmount")]
        public decimal price { get; set; }

        [NotMapped]
        public IFormFile image { get; set; }
        public int Type { get; set; }
        public int CategoryID { get; set; }
        public long SellerID { get; set; }
        public string MainPicture { get; set; }
        public string SmallPicture { get; set; }
        public string Title { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MinimumAmount { get; set; }
        public int IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; }
    }
}