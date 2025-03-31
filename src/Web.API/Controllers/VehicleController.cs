using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.API.Models;
using Web.API.Services;

namespace Web.API.Controllers
{
    [Route("api/vehicle")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<VehicleDTO>> AddVehicle(VehicleDTO vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var addedVehicle = await _vehicleService.AddVehicleAsync(vehicle.FromDTO());
                return CreatedAtAction(nameof(GetVehicle), new { id = addedVehicle.ID }, addedVehicle.ToDTO());
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
        [HttpPut("update")]
        public async Task<ActionResult<VehicleDTO>> UpdateVehicle(VehicleDTO vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var addedVehicle = await _vehicleService.UpdateVehicleAsync(vehicle.FromDTO());
                return CreatedAtAction(nameof(GetVehicle), new { id = addedVehicle.ID }, addedVehicle.ToDTO());
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
        //This method uses POST instead of GET because of search parameters
        [HttpPost]
        public async Task<ActionResult<IEnumerable<VehicleDTO>>> GetVehicles(
            [FromBody] Request vehicleRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var vehicles = await _vehicleService.GetVehiclesAsync(vehicleRequest);
                var response = vehicles.Select(v => v.ToDTO()).ToList();
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
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
        public async Task<ActionResult<VehicleDTO>> GetVehicle(string id)
        {
            try
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
                var response = vehicle.ToDTO();
                return Ok(response);
            }
            catch (Exception)
            {
                return Problem();
            }
        }
        [HttpPost("delete/{id}")]
        public async Task<ActionResult<VehicleDTO>> DeleteVehicle(string id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("ID is required.");
                }

                var vehicle = await _vehicleService.DeleteVehicleByIdAsync(id);

                if (vehicle == null)
                {
                    return NotFound($"Vehicle with ID {id} not found.");
                }
                var response = vehicle.ToDTO();
                return Ok(response);
            }
            catch (Exception)
            {
                return Problem();
            }
        }
    }
}
