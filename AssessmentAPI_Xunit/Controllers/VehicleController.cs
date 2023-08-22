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
                if (vehicletype != null)
                {
                    vehicletype.VehicleTypeId = new int();
                    var result = await vehicleinterface.AddVehicleType(vehicletype);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return NotFound();
                    }

                }
                else
                {
                    return BadRequest();
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
                if (id != vehicletype.VehicleTypeId)
                {
                    return BadRequest();
                }
                var istrue = vehicleinterface.IsExists(id);
                if (istrue==true)
                {
                    var success=await vehicleinterface.UpdateVehicleType(id, vehicletype);
                    return Ok("Success");
                }
                return BadRequest("Id not found");

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