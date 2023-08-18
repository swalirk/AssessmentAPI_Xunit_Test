using AssessmentAPI_Xunit.Controllers;
using AssessmentAPI_Xunit.model;
using AssessmentAPI_Xunit.Service.Interface;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Org.BouncyCastle.Crypto;
using System.Collections.Generic;

namespace TestProject.Controller
{
    public class TestBrandController
    {

        private readonly IFixture fixture;
        private readonly Mock<IBrandInteface> brandInterface;
        private readonly Mock<IVehicleInterface> vehicleInterface;
        private readonly BrandController brandController;

        public TestBrandController()
        {
            fixture = new Fixture();
            brandInterface = fixture.Freeze<Mock<IBrandInteface>>();
            vehicleInterface = fixture.Freeze<Mock<IVehicleInterface>>();
            brandController = new BrandController(brandInterface.Object, vehicleInterface.Object);
        }



        [Fact]
        public void AddBrand_ValidInput_ReturnsOkResult()
        {
            // Arrange
            
            var brandData = fixture.Create<Brand>();
            var returnData = fixture.Create<Brand>();
            brandInterface.Setup(c => c.AddBrand(brandData)).ReturnsAsync(returnData);
            // Act
            var result =  brandController.AddBrand(returnData);

            // Assert
            result.Should().NotBeNull();  
            result.Result.Should().BeAssignableTo<OkObjectResult>();
            brandInterface.Verify(t => t.AddBrand(returnData), Times.Once());
        }
        [Fact]
        public void AddBrand_NullInput_ReturnsBadRequest()
        {

            Brand brandData =null;
            brandInterface.Setup(c => c.AddBrand(brandData)).ReturnsAsync((Brand)null);
            // Act
            var result = brandController.AddBrand(brandData);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<BadRequestResult>();
            brandInterface.Verify(t => t.AddBrand(brandData), Times.Never());
        }
        [Fact]

        public void AddBrand_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange

            brandInterface.Setup(repo => repo.AddBrand(It.IsAny<Brand>()))
                .ThrowsAsync(new Exception("Some error message"));


            // Act
            var result =  brandController.AddBrand(new Brand());

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<BadRequestObjectResult>();
            brandInterface.Verify(t => t.AddBrand(new Brand()), Times.Never());

        }
        [Fact]
        public async Task AddBrand_ForeignKeyConstraintFails_ReturnsBadRequest()
        {
            // Arrange
           
            var id = fixture.Create<int>();
            var invalidBrand = fixture.Create<Brand>(); 
            brandInterface.Setup(b => b.AddBrand(invalidBrand)).ThrowsAsync(new DbUpdateException("Could not save changes. Please configure your entity type accordingly."));

            // Act
            var result = await brandController.AddBrand(invalidBrand);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Could not save changes. Please configure your entity type accordingly.", badRequestResult.Value);
        }

        [Fact]
        public void GetAllBrands_Return_Brands()
        {
            // Arrange
            List<Brand> brandList = fixture.CreateMany<Brand>().ToList();
            brandInterface.Setup(c => c.GetAllBrands()).Returns(brandList);

            // Act
            var result = brandController.GetAllBrands();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var brands = okResult.Value as List<Brand>;
            Assert.NotNull(brands);
            Assert.NotEmpty(brands);
        }

        [Fact]
        public void GetAllBrands_Return_BadRequest_WhenDatanotFound()
        {
            ICollection<Brand> brandList = fixture.CreateMany<Brand>(0).ToList();
            
            brandInterface.Setup(c => c.GetAllBrands()).Returns(brandList);

            // Act
            var result = brandController.GetAllBrands();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Data Not Found", badRequestResult.Value);
        }
        [Fact]
        public void GetAllBrands_Exception_ReturnsBadRequestWithExceptionMessage()
        {


            // Arrange
            brandInterface.Setup(c => c.GetAllBrands()).Throws(new Exception("Something went wrong"));

            // Act
            var result = brandController.GetAllBrands();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Something went wrong", badRequestResult.Value);
        }

        [Fact]
        public void GetAllBrandsOfAVehicleType_ValidId_ReturnsOkResultWithBrands()
        {
            // Arrange
            int vehicleTypeId = fixture.Create<int>();
            var mockBrands = new List<Brand> { new Brand(), new Brand() };
            brandInterface.Setup(x => x.GetAllBrandsOfAVehicleType(vehicleTypeId)).Returns(mockBrands);
            vehicleInterface.Setup(x => x.IsExists(vehicleTypeId)).Returns(true);

            // Act
            var result = brandController.GetAllBrandsOfAVehicleType(vehicleTypeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBrands = Assert.IsAssignableFrom<IEnumerable<Brand>>(okResult.Value);
            Assert.Equal(mockBrands.Count, returnedBrands.Count());
        }

        [Fact]
        public void GetAllBrandsOfAVehicleType_InvalidId_ReturnsBadRequestWithMessage()
        {
            // Arrange
            int vehicleTypeId =fixture.Create<int>();
            vehicleInterface.Setup(repo => repo.IsExists(vehicleTypeId)).Returns(false);

            // Act
            var result = brandController.GetAllBrandsOfAVehicleType(vehicleTypeId);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Id not found", badRequestResult.Value);
            vehicleInterface.Verify(repo => repo.IsExists(vehicleTypeId),Times.Once());
        }
        [Fact]
        public void GetAllBrandsOfAVehicleType_NoBrands_ReturnsBadRequest()
        {
            // Arrange
            int vehicleTypeId = fixture.Create<int>();
            List<Brand> emptyBrands = new List<Brand>();

            vehicleInterface.Setup(v => v.IsExists(vehicleTypeId)).Returns(true);
            brandInterface.Setup(b => b.GetAllBrandsOfAVehicleType(vehicleTypeId)).Returns(emptyBrands);

            // Act
            var result = brandController.GetAllBrandsOfAVehicleType(vehicleTypeId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Data Not Found", badRequestResult.Value);
            vehicleInterface.Verify(x => x.IsExists(vehicleTypeId), Times.Once());
        }
        [Fact]
        public void GetAllBrandsByVehicleType_ShouldReturnBadResponse_WhenVehicleIdIsNotExists()
        {
            // Arrange
            int id = fixture.Create<int>();
            ICollection<Brand> brandByVehicleTypeList = null;
            brandInterface.Setup(x => x.GetAllBrandsOfAVehicleType(id)).Returns(brandByVehicleTypeList);
            vehicleInterface.Setup(x => x.IsExists(id)).Returns(false);

            // Act
            var result = brandController.GetAllBrandsOfAVehicleType(id);

            // Assert
            result.Should().NotBeNull();
            var getResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            brandInterface.Verify(x => x.GetAllBrandsOfAVehicleType(id), Times.Never());
            vehicleInterface.Verify(x => x.IsExists(id), Times.Once());
        }

        [Fact]
        public void DeleteBrand_ValidId_ReturnsOkResult()
        {
            // Arrange
            
            var brand = fixture.Create<Brand>();
            brandInterface.Setup(x => x.DeleteBrand(brand.BrandId)).Returns(true);
            brandInterface.Setup(x => x.IsExists(brand.BrandId)).Returns(true);
            

            // Act
            var result = brandController.DeleteBrand(brand.BrandId);

            // Assert
            result.Should().NotBeNull();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted", okResult.Value);
            brandInterface.Verify(x => x.IsExists(brand.BrandId), Times.Once());
        }
        [Fact]
        public void DeleteBrand_InvalidId_ReturnsBadRequestWithMessage()
        {
            // Arrange
            
            var brand = fixture.Create<Brand>();
            brandInterface.Setup(x => x.DeleteBrand(brand.BrandId)).Returns(false);
            brandInterface.Setup(x => x.IsExists(brand.BrandId)).Returns(false);

            // Act
            var result = brandController.DeleteBrand(brand.BrandId);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something Went Wrong", badRequestResult.Value);
            brandInterface.Verify(x => x.IsExists(brand.BrandId) ,Times.Once());
        }
        [Fact]
        public void DeleteBrand_Exception_ReturnsBadRequestWithExceptionMessage()
        {
            // Arrange
            var brand=fixture.Create<Brand>();
            var exceptionMessage = "An error occurred.";
            brandInterface.Setup(repo => repo.IsExists(brand.BrandId)).Returns(true);
            brandInterface.Setup(repo => repo.DeleteBrand(brand.BrandId))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = brandController.DeleteBrand(brand.BrandId);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
            brandInterface.Verify(x => x.IsExists(brand.BrandId), Times.Once());
        }



        [Fact]
        public async Task UpdateBrand_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            int id = fixture.Create<int>();
            var updateBrand = fixture.Create<Brand>();
            var existing = fixture.Create<Brand>();
            
            brandInterface.Setup(b => b.GetBrandById(id)).Returns(existing);
            brandInterface.Setup(b => b.UpdateBrand(id, updateBrand, existing)).ReturnsAsync(true);

            // Act
            var result = await brandController.UpdateBrand(id, updateBrand);

            // Assert
            result.Should().NotBeNull();
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal("Success", okResult.Value);
        }

        [Fact]
        public void UpdateBrand_ShouldReturnBadRequestResult_WhenUpdateFails()
        {
            // Arrange
            int id = fixture.Create<int>();
            var updateBrand = fixture.Create<Brand>();
            var existing = fixture.Create<Brand>();
            brandInterface.Setup(c => c.UpdateBrand(id, updateBrand, existing)).ReturnsAsync(false);

            // Act
            var result = brandController.UpdateBrand(id, updateBrand);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Task<IActionResult>>();
            result.Result.Should().BeAssignableTo<BadRequestResult>();
        }

        [Fact]
        public async Task UpdateBrand_ShouldReturnBadRequestObjectResult_WhenAnExceptionOccurred()
        {
            // Arrange
            int id = fixture.Create<int>();
            var updateBrand = fixture.Create<Brand>();
            var existing = fixture.Create<Brand>();
            brandInterface.Setup(b => b.GetBrandById(id)).Returns(existing);
            brandInterface.Setup(b => b.UpdateBrand(id, updateBrand, existing)).ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await brandController.UpdateBrand(id, updateBrand);

            // Assert
            result.Should().NotBeNull();
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Something went wrong", badRequestResult.Value);
        }
        [Fact]
        public async Task UpdateBrand_ShouldReturnBadRequest_WhenForeignKeyConstraintFails()
        {
            // Arrange
            int id = fixture.Create<int>();
            var updateBrand = fixture.Create<Brand>();
            var existing = fixture.Create<Brand>();

            brandInterface.Setup(b => b.GetBrandById(id)).Returns(existing);
            brandInterface.Setup(b => b.UpdateBrand(id, updateBrand, existing)).ThrowsAsync(new DbUpdateException("Could not save changes. Please configure your entity type accordingly."));

            // Act
            var result = await brandController.UpdateBrand(id, updateBrand);

            // Assert
            result.Should().NotBeNull();
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Could not save changes. Please configure your entity type accordingly.", badRequestResult.Value);
        }


    }





}
