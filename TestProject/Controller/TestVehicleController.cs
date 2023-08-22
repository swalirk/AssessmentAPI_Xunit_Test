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
            // arrange
            var vehicleData = fixture.Create<VehicleType>();
            var returnData = fixture.Create<VehicleType>();
            vehicleInterface.Setup(c => c.AddVehicleType(vehicleData)).ReturnsAsync(returnData);
           
            //act
            var result = vehicleController.AddVehicleType(vehicleData);

            // Assert
            result.Should().NotBeNull();
            var okObjectResult = result.Result.As<OkObjectResult>();
            okObjectResult.Value.Should().BeEquivalentTo(returnData);
            result.Result.Should().BeAssignableTo<OkObjectResult>();
            vehicleInterface.Verify(c => c.AddVehicleType(vehicleData), Times.Once());
        }
        [Fact]
        public async Task AddVehicleType_NullInput_ReturnsBadRequest()
        {
           
            VehicleType vehicleData = null;
            vehicleInterface.Setup(c => c.AddVehicleType(vehicleData)).ReturnsAsync((VehicleType)null);
            // Act
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
            var vehicletype=fixture.Create<VehicleType>();
            vehicleInterface.Setup(repo => repo.AddVehicleType(vehicletype))
                .ThrowsAsync(new Exception("Some error message"));
            // Act
            var result = await vehicleController.AddVehicleType(vehicletype);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Some error message", badRequestResult.Value);
            vehicleInterface.Verify(t => t.AddVehicleType(vehicletype), Times.Once());
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
            Assert.Equal(vehicleMock.Count, returnedData.Count);
            vehicleInterface.Verify(vi => vi.GetAllVehicleTypes(),Times.Once());


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
            vehicleInterface.Verify(vi => vi.GetAllVehicleTypes(), Times.Once());
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
            vehicleInterface.Verify(vi => vi.GetAllVehicleTypes(), Times.Once());
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            
        }
    

        [Fact]
        public async Task UpdateVehicleType_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange

            int id = fixture.Create<int>();
            var updateVehicleType = fixture.Create<VehicleType>();
            updateVehicleType.VehicleTypeId = id;
            vehicleInterface.Setup(v=>v.IsExists(id)).Returns(true);
            vehicleInterface.Setup(v => v.UpdateVehicleType(id, updateVehicleType)).ReturnsAsync(true);
            // Act
            var result = await vehicleController.UpdateVehicleType(id, updateVehicleType);

            // Assert
            result.Should().NotBeNull();
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal("Success", okResult.Value);
            vehicleInterface.Verify(v => v.IsExists(id),Times.Once);
            vehicleInterface.Verify(v => v.UpdateVehicleType(id, updateVehicleType),Times.Once());
        }

       
        [Fact]
        public void UpdateVehicletype_InvalidData_ReturnsBadRequest()
        {
            // Arrange

            int id = fixture.Create<int>();
            var updatedVehicletype = fixture.Create<VehicleType>();
          
            vehicleInterface.Setup(service => service.UpdateVehicleType(id, updatedVehicletype)).ReturnsAsync(false);

            // Act
            var result =  vehicleController.UpdateVehicleType(id, updatedVehicletype);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Task<IActionResult>>();
            result.Result.Should().BeAssignableTo<BadRequestResult>();
            vehicleInterface.Verify(v => v.UpdateVehicleType(id, updatedVehicletype), Times.Never());
        }
        [Fact]
        public async void UpdateVehicleType_ShouldReturnBadRequestResponse_WhenIdNotInDataBase()
        {
            //Arrange
            int id = fixture.Create<int>();
            var updateVehicletype = fixture.Create<VehicleType>();

            updateVehicletype.VehicleTypeId = id;
            vehicleInterface.Setup(x => x.IsExists(id)).Returns(false);
            vehicleInterface.Setup(x => x.UpdateVehicleType(id,updateVehicletype)).ReturnsAsync(false);
            

            //Act
            var result = await vehicleController.UpdateVehicleType(id,updateVehicletype);
            //Assert
            result.Should().NotBeNull();
  
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            vehicleInterface.Verify(x=>x.IsExists(id),Times.Once);
            vehicleInterface.Verify(x => x.UpdateVehicleType(id, updateVehicletype), Times.Never());

        }
        [Fact]
        public async Task UpdateVehicleType_ShouldReturnBadRequestObjectResult_WhenAnExceptionOccurred()
        {
            // Arrange
            int id = fixture.Create<int>();
            var updateVehicleType = fixture.Create<VehicleType>();

            updateVehicleType.VehicleTypeId = id;
            vehicleInterface.Setup(v => v.IsExists(id)).Returns(true);
            vehicleInterface.Setup(v => v.UpdateVehicleType(id, updateVehicleType)).ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result =await vehicleController.UpdateVehicleType(id, updateVehicleType);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Something went wrong", badRequestResult.Value);
            vehicleInterface.Verify(x => x.IsExists(id), Times.Once);
            vehicleInterface.Verify(x => x.UpdateVehicleType(id, updateVehicleType), Times.Once());
        }
      

    }
}
