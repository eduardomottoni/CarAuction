using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.API.Models;
using Web.API.Services;

namespace Web.API.Controllers
{
    [Route("api/auction")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        public AuctionsController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<AuctionDTO>> StartAuction([FromBody] StartAuctionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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

        [HttpPost("placebid")]
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

        [HttpPost("close")]
        public async Task<ActionResult<AuctionDTO>> CloseAuction([FromBody] CloseAuctionRequest request)
        {
            try
            {
                var auction = await _auctionService.CloseAuctionAsync(request.AuctionId);
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
        [HttpPost("active")]
        public async Task<ActionResult<AuctionDTO>> ActiveAuction([FromBody] ActiveAuctionRequest request)
        {
            try
            {
                var auction = await _auctionService.ActiveAuctionAsync(request.AuctionId);
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
        [HttpPost("delete")]
        public async Task<ActionResult<AuctionDTO>> DeleteAuction([FromBody] DeleteAuctionRequest request)
        {
            try
            {
                var auction = await _auctionService.DeleteAuctionAsync(request.AuctionId);
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
        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuction(string? id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("Id is required");
                }
                var auction = await _auctionService.GetAuctionByIdAsync(id);
                return Ok(auction.ToDto());
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Problem();
            }
        }
        // I could use POST instead of GET if the request allows query
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<AuctionDTO>>> GetAuctions()
        {
            try
            {
                var auctions = await _auctionService.GetAuctionsAsync();
                var response = auctions.Select(v => v.ToDto()).ToList();
                return Ok(response);
            }
            catch (Exception)
            {
                return Problem();
            }
        }
    }
    }
