using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToysBids.AuctionsService.Models;

namespace ToysBids.AuctionsService.Data
{
    public class AuctionsContext : DbContext
    {
        public AuctionsContext(DbContextOptions<AuctionsContext> options) : base(options)
        {
        }
        public DbSet<AuctionBundle> AuctionBundle { get; set; }
        public DbSet<Publication> Publication { get; set; }
    }
}