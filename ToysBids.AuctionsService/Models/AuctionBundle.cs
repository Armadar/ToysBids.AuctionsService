using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToysBids.AuctionsService.Models
{
    public class AuctionBundle
    {
        public long ID { get; set; }
        public long StoreID { get; set; }
        public int CategoryID { get; set; }
        public string Title { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
    }
}