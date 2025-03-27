using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.API.Models;
using Web.API.Services;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        // POST: api/Vehicle
        [HttpPost]
        public async Task<ActionResult<VehicleDTO>> AddVehicle(Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var addedVehicle = await _vehicleService.AddVehicleAsync(vehicle);
                return CreatedAtAction(nameof(GetVehicle), new { id = addedVehicle.ID }, addedVehicle);
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

        // GET: api/Vehicle
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleDTO>>> GetVehicles(
            [FromQuery] string? manufacturer = null,
            [FromQuery] string? type = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? year = null)
        {
            var vehicles = await _vehicleService.GetVehiclesAsync(manufacturer, type, minPrice, maxPrice, year);

            if (!vehicles.Any())
                return NotFound("No vehicles found matching the criteria.");

            return Ok(vehicles.Select(v=>v.ToDTO());
        }

        // GET: api/Vehicle/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDTO>> GetVehicle(string id)
        {
            if (id == null)
            {
                return BadRequest("ID is required.");
            }

            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);

            if (vehicle == null)
            {
                return NotFound($"Vehicle with ID {id} not found.");
            }

            return Ok(vehicle.ToDTO());
        }
    }
}
