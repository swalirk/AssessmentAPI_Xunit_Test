using AssessmentAPI_Xunit.model;
using AssessmentAPI_Xunit.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto;

namespace AssessmentAPI_Xunit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandInteface brandinterface;
        private readonly IVehicleInterface vehicleInterface;



        public BrandController(IBrandInteface brandinterface, IVehicleInterface vehicleInterface)
        {
            this.brandinterface = brandinterface;
            this.vehicleInterface = vehicleInterface;
        }

        [HttpGet]
        [Route("[controller]/GetAllBrands")]
        public ActionResult GetAllBrands()
        {
            try
            {


                var brands = brandinterface.GetAllBrands();
                if (brands.Count > 0)
                {
                    return Ok(brands);

                }
                else
                {
                    return BadRequest("Data Not Found");
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/GetAllBrandsOfAVehicleType/{id}")]
        public ActionResult GetAllBrandsOfAVehicleType([FromRoute] int id)
        {
            try
            {
                if (vehicleInterface.IsExists(id))
                {
                    var brandsByVehicleType = brandinterface.GetAllBrandsOfAVehicleType(id);
                    if (brandsByVehicleType.Count > 0)
                    {
                        return Ok(brandsByVehicleType);

                    }
                    else
                    {
                        return BadRequest("Data Not Found");
                    }
                }
                else
                {
                    return BadRequest("Id not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]

        public async Task<ActionResult> AddBrand([FromBody] Brand brand)
        {


            try
            {
                if (brand != null)
                {
                    brand.BrandId = new int();
                    var result = await brandinterface.AddBrand(brand);
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
        public async Task<IActionResult> UpdateBrand(int id, [FromBody]Brand brand)
        {
            try
            {
               
                if(id!=brand.BrandId)
                {
                    return BadRequest();
                }
                if(brandinterface.IsExists(id))
                {
                     await brandinterface.UpdateBrand(id, brand);
                    return Ok("Success");
                }
                return BadRequest("Id not found");
                

                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpDelete]
        [Route("[controller]/DeleteBrand/{id}")]
        public ActionResult DeleteBrand([FromRoute] int id)
        {
            try
            {
                if (brandinterface.IsExists(id))
                {
                    brandinterface.DeleteBrand(id);
                    return Ok("Deleted");
                }
                else
                {
                    return BadRequest("Something Went Wrong");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




    }
}
