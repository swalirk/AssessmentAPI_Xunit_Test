using AssessmentAPI_Xunit.model;
using AssessmentAPI_Xunit.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;

namespace AssessmentAPI_Xunit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleInterface vehicleinterface;


        public VehicleController(IVehicleInterface vehicleInterface)
        {
            this.vehicleinterface = vehicleInterface;

        }


        [HttpPost]
        [Route("[controller]/AddVehicleType")]
        public async Task<ActionResult> AddVehicleType([FromBody] VehicleType vehicletype)
        {
            try
            {
                if (vehicletype == null)
                {
                    return BadRequest();

                }

                else
                {
                    vehicletype.VehicleTypeId = new int();
                    var vehicleTypeIsAdded = await vehicleinterface.AddVehicleType(vehicletype);
                    return Ok(vehicletype);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicleType(int id, [FromBody]VehicleType vehicletype)
        {
            try
            {

                var existingtype = vehicleinterface.GetVehicleTypeById(id);
                var success = await vehicleinterface.UpdateVehicleType(id, vehicletype, existingtype);
                if (success == false)
                {
                    return BadRequest();
                }

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet]
        [Route("[controller]/GetAllVehicleTypes")]
        public IActionResult GetAllVehicleTypes()
        {
            try
            {
                var vehicleTypes = vehicleinterface.GetAllVehicleTypes();

                if (vehicleTypes.Count == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(vehicleTypes);

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}