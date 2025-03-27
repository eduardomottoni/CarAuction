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
        public async Task<ActionResult<AuctionDTO>> StartAuction([FromBody] StartAuctionRequest request)
        {
            try
            {
                var auction = await _auctionService.StartAuctionAsync(request.VehicleId, request.StartDate, request.EndDate, request.Active, request.AuctionId, request.StartBid);
                return CreatedAtAction(nameof(GetAuction), new { id = auction.ID }, auction);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Problem();
            }
        }

        // POST: api/Auctions/PlaceBid
        [HttpPost("PlaceBid")]
        public async Task<ActionResult<AuctionDTO>> PlaceBid([FromBody] PlaceBidRequest request)
        {
            try
            {
                var auction = await _auctionService.PlaceBidAsync(request.AuctionId, request.BidAmount);
                return Ok(auction.ToDto());
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Problem();
            }
        }

        // POST: api/Auctions/Close
        [HttpPost("Close")]
        public async Task<ActionResult<AuctionDTO>> CloseAuction([FromBody] CloseAuctionRequest request)
        {
            try
            {
                var auction = await _auctionService.CloseAuctionAsync(request.AuctionId);
                return Ok(auction.ToDto());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // POST: api/Auctions/Close
        [HttpPost("Delete")]
        public async Task<ActionResult<AuctionDTO>> DeleteAuction([FromBody] DeleteAuctionRequest request)
        {
            try
            {
                var auction = await _auctionService.DeleteAuctionAsync(request.AuctionId);
                return Ok(auction.ToDto());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // GET: api/Auctions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuction(string? id)
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
            return Ok(auction.ToDto());
        }
        // GET: api/Auctions
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<AuctionDTO>>> GetAuctions()
        {
            var auctions = await _auctionService.GetAuctionsAsync();
            if(auctions == null)
            {
                return NotFound();
            }
            
            return Ok(auctions.Select(v=>v.ToDto()));
        }
    }

    public class StartAuctionRequest
    {
        public string VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Active { get; set; }
        public string? AuctionId { get; set; }
        public decimal? StartBid { get; set; }
    }

    public class DeleteAuctionRequest
    {
        public string AuctionId { get; set; }
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
