using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToysBids.AuctionsService.Data;
using ToysBids.AuctionsService.Handlers;
using ToysBids.AuctionsService.Models;

namespace ToysBids.AuctionsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionsContext _context;
        private readonly IImageHandler _imageHandler;

        public AuctionsController(IImageHandler imageHandler, AuctionsContext context)
        {
            _imageHandler = imageHandler;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<AuctionBundle>> PostAuctionBundle([FromForm]AuctionBundle auctionBundle)
        {
            dynamic r = new System.Dynamic.ExpandoObject();

            try
            {
                auctionBundle.From = DateTime.Now;
                auctionBundle.CreatedOn = DateTime.Now;
                _context.AuctionBundle.Add(auctionBundle);
                await _context.SaveChangesAsync();
                r.auctionBundleId = auctionBundle.ID.ToString();
            }
            catch (Exception ex)
            {
                r.message = ex.Message;
                //throw;
            }
            //TODO: Return an error object ?
            return new ObjectResult(r);
        }
        // GET: api/Auctions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionBundle>>> GetAuctionBundles()
        {
            //TODO: Check performance:
            //            https://social.msdn.microsoft.com/Forums/en-US/30b6e9c6-d414-4b1d-9fb1-b5ff5f455ea9/stored-procedure-or-linq-for-million-of-records-with-ef-core-20?forum=linqtosql
            // TODO: Take in mind use raw SQL: https://docs.microsoft.com/en-us/ef/core/querying/raw-sql
            //TODO: Check new GroupBy feature from EF 2.1: https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-2-1-preview-2/
            //TODO: use pagination
            var query = (from auctionBundle in _context.AuctionBundle
                         join auction in _context.Publication
                         on auctionBundle.ID equals auction.auctionBundleId
                         group new { auctionBundle, auction } by new { auctionBundle.ID, auctionBundle.StoreID, auctionBundle.CategoryID, auctionBundle.Title, auctionBundle.From, auctionBundle.To, auctionBundle.CreatedOn, auctionBundle.CreatedBy } into newGroup
                         select new
                         {
                             newGroup.Key.ID,
                             newGroup.Key.StoreID,
                             newGroup.Key.CategoryID,
                             newGroup.Key.Title,
                             newGroup.Key.From,
                             newGroup.Key.To,
                             newGroup.Key.CreatedOn,
                             newGroup.Key.CreatedBy,
                             auctionsCount = newGroup.Count()
                         }).ToList();
            List<AuctionBundle> r = new List<AuctionBundle>();
            query.ForEach(item =>
            r.Add(new AuctionBundle()
            {
                ID = item.ID,
                StoreID = item.StoreID,
                CategoryID = item.CategoryID,
                Title = item.Title,
                From = item.From,
                To = item.To,
                CreatedOn = item.CreatedOn,
                CreatedBy = item.CreatedBy,
                auctionsCount = item.auctionsCount
            })
            );
            //return await _context.AuctionBundle.ToListAsync();
            return r;
        }
        // GET: api/Auctions/5
        [HttpGet("getauctionsbyauctionbundleid/{id}")]
        public async Task<ActionResult<IEnumerable<Publication>>> GetAuctionsByAuctionBundleId(long id)
        {
            try
            {
                var auctions = await _context.Publication.Where(x => x.auctionBundleId == id).ToListAsync();

                if (auctions == null)
                {
                    return NotFound();
                }

                return auctions;
            }
            catch (Exception ex)
            {
                var err = ex;
                throw;
            }

        }
        // GET: api/Auctions/5
        [HttpGet("getauctionInfo/{id}")]
        public async Task<ActionResult<Publication>> GetAuctionInfo(long id)
        {
            var exchange = await _context.Publication.FindAsync(id);

            if (exchange == null)
            {
                return NotFound();
            }

            return exchange;
        }
        //TODO Use PUT instead of Get
        [HttpGet("updateauction/{id}/{baseprice}")]
        public async Task<IActionResult> UpdateAuction(long id, decimal basePrice)
        {
            if (id ==0 || basePrice == 0)
            {
                return BadRequest();
            }

            var publication = await _context.Publication.FindAsync(id);
            publication.price = basePrice;
            _context.Entry(publication).State = EntityState.Modified;

            try
            { 

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                throw;
            }

            return NoContent();
        }

        [HttpPost("uploadauction")]
        public async Task<IActionResult> UploadAuction([FromForm]  Publication auction)
        {            
            try
            {
                string name = Guid.NewGuid().ToString();
                var x = await _imageHandler.UploadImage(auction.image, name);

                auction.MainPicture = "http://localhost/images/" + name+".jpg";
                auction.SmallPicture = auction.MainPicture;
                auction.Title = string.Empty;
                auction.BeginDate = DateTime.Now;
                auction.MinimumAmount = 2;
                auction.IsActive = 1;
                auction.CreatedBy = Convert.ToInt32(auction.SellerID);
                auction.CreatedOn = DateTime.Now;

                _context.Publication.Add(auction);
                await _context.SaveChangesAsync();

                return x;
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                throw;
            }            
        }

        // GET: api/Auctions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionBundle>> GetAuctionBundle(long id)
        {
            var exchange = await _context.AuctionBundle.FindAsync(id);

            if (exchange == null)
            {
                return NotFound();
            }

            return exchange;
        }
    }
}