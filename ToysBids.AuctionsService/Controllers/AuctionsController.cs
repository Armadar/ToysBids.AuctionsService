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
                r.message = auctionBundle.ID.ToString();
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
            return await _context.AuctionBundle.ToListAsync();
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
        public AuctionsController(IImageHandler imageHandler,AuctionsContext context)
        {
            _imageHandler = imageHandler;
            _context = context;
        }
        [HttpPost("uploadauction")]
        public async Task<IActionResult> UploadAuction([FromForm]  Publication auction)
        {            
            try
            {
                string name = Guid.NewGuid().ToString();
                var x = await _imageHandler.UploadImage(auction.data, name);

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