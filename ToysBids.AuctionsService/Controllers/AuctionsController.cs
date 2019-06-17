﻿using System;
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

        // GET: api/Exchanges
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionBundle>>> GetAuctionBundles()
        {
            return await _context.AuctionBundle.ToListAsync();
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
                auction.CreatedBy = auction.SellerID;
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

        // GET: api/Exchanges/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionBundle>> GetExchange(long id)
        {
            var exchange = await _context.AuctionBundle.FindAsync(id);

            if (exchange == null)
            {
                return NotFound();
            }

            return exchange;
        }

        // POST: api/Exchanges
        [HttpPost]
        public async Task<ActionResult<AuctionBundle>> PostAuctionBundle([FromForm]AuctionBundle auctionBundle)
        {
            
            try
            {
                string title = "";
                switch (auctionBundle.CategoryID)
                {
                    case 1:  title = "Transformers"; break;
                    case 2: title = "Star Wars"; break;
                    case 3: title = "Marvel"; break;
                }

                auctionBundle.Title = "Subasta - " + title;
                auctionBundle.CreatedOn = DateTime.Now;

                _context.AuctionBundle.Add(auctionBundle);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                throw;
            }



            dynamic r = new System.Dynamic.ExpandoObject();
            r.message = auctionBundle.ID.ToString();

            return new ObjectResult(r);
        }
    }
}