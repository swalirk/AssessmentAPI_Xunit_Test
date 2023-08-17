using AssessmentAPI_Xunit.Controllers;
using AssessmentAPI_Xunit.model;
using AssessmentAPI_Xunit.Service.Interface;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestProject.Controller
{
    public class TestVehicleController
    {
        private readonly IFixture fixture;
        private readonly Mock<IBrandInteface> brandInterface;
        private readonly Mock<IVehicleInterface> vehicleInterface;
        private readonly VehicleController vehicleController;
        public TestVehicleController()
        {
            this.fixture = new Fixture();
            vehicleInterface = fixture.Freeze<Mock<IVehicleInterface>>();
            vehicleController = new VehicleController( vehicleInterface.Object);
        }


        [Fact]
        public async Task AddVehicle_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var vehicle = new VehicleType();
            vehicleInterface.Setup(repo => repo.AddVehicleType(It.IsAny<VehicleType>()))
                               .ReturnsAsync(vehicle);

            var controller = new VehicleController(vehicleInterface.Object);

            // Act
            var result = await controller.AddVehicleType(vehicle);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(vehicle, okResult.Value);
        }
        [Fact]
        public async Task AddVehicleType_NullInput_ReturnsBadRequest()
        {
           

            // Act
            var result = await vehicleController.AddVehicleType(null) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
       
        public async Task AddVehicle_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange

            vehicleInterface.Setup(repo => repo.AddVehicleType(It.IsAny<VehicleType>()))
                .ThrowsAsync(new Exception("Some error message"));

            var controller = new VehicleController(vehicleInterface.Object);

            // Act
            var result = await controller.AddVehicleType(new VehicleType());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Some error message", badRequestResult.Value);
        }


        [Fact]
        public void GetAllVehicleTypes_ValidData_ReturnsOkWithData()
        {
            // Arrange
            
            var vehicleTypesData = fixture.Create<List<VehicleType>>();

            
            vehicleInterface.Setup(vi => vi.GetAllVehicleTypes()).Returns(vehicleTypesData);

            

            // Act
            var result = vehicleController.GetAllVehicleTypes() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var returnedData = result.Value as List<VehicleType>;
            Assert.NotNull(returnedData);
            Assert.Equal(vehicleTypesData.Count, returnedData.Count);
            // You can add further assertions based on your application logic
        }


        [Fact]
        public void GetAllVehicleTypes_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            
            vehicleInterface.Setup(vi => vi.GetAllVehicleTypes()).Throws(new Exception("Test Exception"));

            

            // Act
            var result = vehicleController.GetAllVehicleTypes() as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Test Exception", result.Value);
        }


        [Fact]
        public void GetAllVehicleTypes_ReturnsBadRequest_WhenDataNotFound()
        {
            // Arrange
            
            vehicleInterface.Setup(vi => vi.GetAllVehicleTypes()).Returns(new List<VehicleType>());

            

            // Act
            var result = vehicleController.GetAllVehicleTypes() as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        

        [Fact]
        public async Task UpdateVehicleType_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            int id = 1;
            var updateVehicleType = new VehicleType { VehicleTypeId = id, TypeName = "Updated Type" };
            var existingType = new VehicleType { VehicleTypeId = id, TypeName = "Existing Type" };

           
            vehicleInterface.Setup(v => v.GetVehicleTypeById(id)).Returns(existingType);
            vehicleInterface.Setup(v => v.UpdateVehicleType(id, updateVehicleType, existingType)).ReturnsAsync(true);

           

            // Act
            var result = await vehicleController.UpdateVehicleType(id, updateVehicleType);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal("Success", okResult.Value);
        }

       
        [Fact]
        public async Task UpdateVehicletype_InvalidData_ReturnsBadRequest()
        {
            // Arrange
  
            var id = 0;
            var updatedVehicletype = new VehicleType
            {
                VehicleTypeId = 6,
                TypeName = "GHJ",
                Description = "SSJSJS",
                IsActive = true
            };
            var existingtype = new VehicleType { VehicleTypeId = id, TypeName = "Updated Type" };
            vehicleInterface.Setup(service => service.UpdateVehicleType(id, updatedVehicletype,existingtype)).ReturnsAsync(false);

            // Act
            var result = await vehicleController.UpdateVehicleType(id, updatedVehicletype);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public async Task UpdateVehicleType_ShouldReturnBadRequestObjectResult_WhenAnExceptionOccurred()
        {
            // Arrange
            int id = 1;
            var updateVehicleType = new VehicleType { VehicleTypeId = id, TypeName = "Updated Type" };
            var existingType = new VehicleType { VehicleTypeId = id, TypeName = "Existing Type" };

            
            vehicleInterface.Setup(v => v.GetVehicleTypeById(id)).Returns(existingType);
            vehicleInterface.Setup(v => v.UpdateVehicleType(id, updateVehicleType, existingType)).ThrowsAsync(new Exception("Something went wrong"));

            

            // Act
            var result = await vehicleController.UpdateVehicleType(id, updateVehicleType);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Something went wrong", badRequestResult.Value);
        }
      

    }
}