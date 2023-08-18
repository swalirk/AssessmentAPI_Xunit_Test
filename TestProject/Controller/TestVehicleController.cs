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
            // Act
            var vehicleData = fixture.Create<VehicleType>();
            var returnData = fixture.Create<VehicleType>();
            vehicleInterface.Setup(c => c.AddVehicleType(vehicleData)).ReturnsAsync(returnData);
           
            var result = vehicleController.AddVehicleType(returnData);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<OkObjectResult>();
            vehicleInterface.Verify(t => t.AddVehicleType(returnData), Times.Once());
        }
        [Fact]
        public async Task AddVehicleType_NullInput_ReturnsBadRequest()
        {
            // Act
            VehicleType vehicleData = null;
            vehicleInterface.Setup(c => c.AddVehicleType(vehicleData)).ReturnsAsync((VehicleType)null);
            var result = vehicleController.AddVehicleType(vehicleData);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<BadRequestResult>();
           vehicleInterface.Verify(t => t.AddVehicleType(vehicleData), Times.Never());
        }

        [Fact]
       
        public async Task AddVehicle_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange

            vehicleInterface.Setup(repo => repo.AddVehicleType(It.IsAny<VehicleType>()))
                .ThrowsAsync(new Exception("Some error message"));
            // Act
            var result = await vehicleController.AddVehicleType(new VehicleType());

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Some error message", badRequestResult.Value);
        }


        [Fact]
        public void GetAllVehicleTypes_ValidData_ReturnsOkWithData()
        {
            // Arrange
            
            var vehicleMock = fixture.Create<ICollection<VehicleType>>();
       
            vehicleInterface.Setup(vi => vi.GetAllVehicleTypes()).Returns(vehicleMock);

            // Act
            var result = vehicleController.GetAllVehicleTypes() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedData = result.Value as List<VehicleType>;
            Assert.NotNull(returnedData);
            Assert.Equal(vehicleMock.Count, returnedData.Count);
            
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
            var vehicleList = fixture.CreateMany<VehicleType>(0).ToList();
            vehicleInterface.Setup(vi => vi.GetAllVehicleTypes()).Returns(vehicleList);
            // Act
            var result = vehicleController.GetAllVehicleTypes() as NotFoundResult;

            // Assert
            Assert.NotNull(result); 
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            
        }
    

        [Fact]
        public async Task UpdateVehicleType_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange

            int id = fixture.Create<int>();
            var updateVehicleType = fixture.Create<VehicleType>();
            var existingType = fixture.Create<VehicleType>();
            vehicleInterface.Setup(v => v.GetVehicleTypeById(id)).Returns(existingType);
            vehicleInterface.Setup(v => v.UpdateVehicleType(id, updateVehicleType, existingType)).ReturnsAsync(true);
            // Act
            var result = await vehicleController.UpdateVehicleType(id, updateVehicleType);

            // Assert
            result.Should().NotBeNull();
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal("Success", okResult.Value);
        }

       
        [Fact]
        public void UpdateVehicletype_InvalidData_ReturnsBadRequest()
        {
            // Arrange

            int id = fixture.Create<int>();
            var updatedVehicletype = fixture.Create<VehicleType>();
            var existingtype = fixture.Create<VehicleType>();
            vehicleInterface.Setup(service => service.UpdateVehicleType(id, updatedVehicletype,existingtype)).ReturnsAsync(false);

            // Act
            var result =  vehicleController.UpdateVehicleType(id, updatedVehicletype);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Task<IActionResult>>();
            result.Result.Should().BeAssignableTo<BadRequestResult>();
        }
        [Fact]
        public async Task UpdateVehicleType_ShouldReturnBadRequestObjectResult_WhenAnExceptionOccurred()
        {
            // Arrange
            int id = fixture.Create<int>();
            var updateVehicleType = fixture.Create<VehicleType>();
            var existingType = fixture.Create<VehicleType>();
            vehicleInterface.Setup(v => v.GetVehicleTypeById(id)).Returns(existingType);
            vehicleInterface.Setup(v => v.UpdateVehicleType(id, updateVehicleType, existingType)).ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result =await vehicleController.UpdateVehicleType(id, updateVehicleType);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Something went wrong", badRequestResult.Value);
        }
      

    }
}