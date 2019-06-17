
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
        public string auctionBundleId { get; set; }
        [Column(name: "StartAmount")]
        public string price { get; set; }

        [NotMapped]
        public IFormFile data { get; set; }
        public int Type { get; set; }
        public int CategoryID { get; set; }
        public long SellerID { get; set; }
        public string MainPicture { get; set; }
    }
}