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
        public async Task AddBrand_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var brand = new Brand();
            brandInterface.Setup(repo => repo.AddBrand(It.IsAny<Brand>()))
                               .ReturnsAsync(brand);

            // Act
            var result = await brandController.AddBrand(brand);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(brand, okResult.Value);
        }
        [Fact]
        public async Task AddBrand_NullInput_ReturnsBadRequest()
        {


            // Act
            var result = await brandController.AddBrand(null) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
        [Fact]

        public async Task AddBrand_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange

            brandInterface.Setup(repo => repo.AddBrand(It.IsAny<Brand>()))
                .ThrowsAsync(new Exception("Some error message"));


            // Act
            var result = await brandController.AddBrand(new Brand());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Some error message", badRequestResult.Value);
        }

        [Fact]
        public void GetAllBrands_Return_Brands()
        {
            // Arrange
            brandInterface.Setup(c => c.GetAllBrands()).Returns(new List<Brand> { new Brand(), new Brand() });

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
            brandInterface.Setup(c => c.GetAllBrands()).Returns(new List<Brand>());

            // Act
            var result = brandController.GetAllBrands();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Data Not Found", badRequestResult.Value);
        }
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
            int vehicleTypeId = 1;
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
            int vehicleTypeId = 1;
            vehicleInterface.Setup(repo => repo.IsExists(vehicleTypeId)).Returns(false);

            // Act
            var result = brandController.GetAllBrandsOfAVehicleType(vehicleTypeId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Id not found", badRequestResult.Value);
        }
        [Fact]
        public void GetAllBrandsByVehicleType_ShouldReturnBadResponse_WhenVehicleIdIsNotExists()
        {
            // Arrange
            int id = 1;
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
            int brandId = 1;
            brandInterface.Setup(repo => repo.IsExists(brandId)).Returns(true);

            // Act
            var result = brandController.DeleteBrand(brandId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted", okResult.Value);
        }
        [Fact]
        public void DeleteBrand_InvalidId_ReturnsBadRequestWithMessage()
        {
            // Arrange
            int brandId = 1;
            brandInterface.Setup(repo => repo.IsExists(brandId)).Returns(false);

            // Act
            var result = brandController.DeleteBrand(brandId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something Went Wrong", badRequestResult.Value);
        }
        [Fact]
        public void DeleteBrand_Exception_ReturnsBadRequestWithExceptionMessage()
        {
            // Arrange
            int brandId = 1;
            var exceptionMessage = "An error occurred.";
            brandInterface.Setup(repo => repo.IsExists(brandId)).Returns(true);
            brandInterface.Setup(repo => repo.DeleteBrand(brandId))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = brandController.DeleteBrand(brandId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
        }






        [Fact]
        public async Task UpdateBrand_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            int id = 1;
            var updateBrand = new Brand { BrandId = id, BrandName = "Updated Brand" };
            var existingBrand = new Brand { BrandId = id, BrandName = "Existing Brand" };


            brandInterface.Setup(b => b.GetBrandById(id)).Returns(existingBrand);
            brandInterface.Setup(b => b.UpdateBrand(id, updateBrand, existingBrand)).ReturnsAsync(true);



            // Act
            var result = await brandController.UpdateBrand(id, updateBrand);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal("Success", okResult.Value);
        }

        [Fact]
        public void UpdateBrand_ShouldReturnBadRequestResult_WhenUpdateFails()
        {
            // Arrange
            int id = 2;
            var brandup = fixture.Create<Brand>();
            var updateBrand = new Brand { BrandId = id, BrandName = "Updated Brand" };
            var existingBrand = new Brand { BrandId = id, BrandName = "Existing Brand" };
            brandInterface.Setup(c => c.UpdateBrand(id, updateBrand, existingBrand)).ReturnsAsync(false);

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
            int id = 1;
            var updateBrand = new Brand { BrandId = id, BrandName = "Updated Brand" };
            var existingBrand = new Brand { BrandId = id, BrandName = "Existing Brand" };


            brandInterface.Setup(b => b.GetBrandById(id)).Returns(existingBrand);
            brandInterface.Setup(b => b.UpdateBrand(id, updateBrand, existingBrand)).ThrowsAsync(new Exception("Something went wrong"));


            // Act
            var result = await brandController.UpdateBrand(id, updateBrand);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Something went wrong", badRequestResult.Value);
        }
        [Fact]
        public async Task UpdateBrand_ShouldReturnBadRequest_WhenForeignKeyConstraintFails()
        {
            // Arrange
            int id = 1;
            var updateBrand = new Brand { BrandId = id, VehicleTypeId = 999, BrandName = "Updat" };
            var existingBrand = new Brand { BrandId = id, BrandName = "Existing Brand" };


            brandInterface.Setup(b => b.GetBrandById(id)).Returns(existingBrand);
            brandInterface.Setup(b => b.UpdateBrand(id, updateBrand, existingBrand)).ThrowsAsync(new DbUpdateException("Could not save changes. Please configure your entity type accordingly."));



            // Act
            var result = await brandController.UpdateBrand(id, updateBrand);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Could not save changes. Please configure your entity type accordingly.", badRequestResult.Value);
        }








    }





}
