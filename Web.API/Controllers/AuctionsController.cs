using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.API.Models;
using Web.API.Services;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        public AuctionsController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        // POST: api/Auctions/Start
        [HttpPost("Start")]
        public async Task<ActionResult<Auction>> StartAuction([FromBody] StartAuctionRequest request)
        {
            try
            {
                var auction = await _auctionService.StartAuctionAsync(request.VehicleId, request.StartDate, request.EndDate, request.AuctionId);
                return CreatedAtAction(nameof(GetAuction), new { id = auction.ID }, auction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Auctions/PlaceBid
        [HttpPost("PlaceBid")]
        public async Task<ActionResult<Auction>> PlaceBid([FromBody] PlaceBidRequest request)
        {
            try
            {
                var auction = await _auctionService.PlaceBidAsync(request.AuctionId, request.BidAmount);
                return Ok(auction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Auctions/Close
        [HttpPost("Close")]
        public async Task<ActionResult<Auction>> CloseAuction([FromBody] CloseAuctionRequest request)
        {
            try
            {
                var auction = await _auctionService.CloseAuctionAsync(request.AuctionId);
                return Ok(auction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Auctions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Auction>> GetAuction(string? id)
        {
            if(id == null)
            {
                return BadRequest("Id is required");
            }
            var auction = await _auctionService.GetAuctionByIdAsync(id);
            if (auction == null)
            {
                return NotFound($"Auction with ID {id} not found.");
            }
            return Ok(auction);
        }
        // GET: api/Auctions
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Auction>>> GetAuctions()
        {
            var auctions = await _auctionService.GetAuctionsAsync();
            if(auctions == null)
            {
                return NotFound();
            }
            return Ok(auctions);
        }
    }

    public class StartAuctionRequest
    {
        public string VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? AuctionId { get; set; }
    }

    public class PlaceBidRequest
    {
        public string AuctionId { get; set; }
        public decimal BidAmount { get; set; }
    }

    public class CloseAuctionRequest
    {
        public string AuctionId { get; set; }
    }
}
