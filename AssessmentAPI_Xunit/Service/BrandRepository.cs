using AssessmentAPI_Xunit.model;
using AssessmentAPI_Xunit.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AssessmentAPI_Xunit.Service
{
    public class BrandRepository : IBrandInteface
    {


        private readonly VehicleBrandContext dbContext;

        public BrandRepository(VehicleBrandContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Brand> AddBrand(Brand brand)
        {
            await dbContext.Brands.AddAsync(brand);

            await dbContext.SaveChangesAsync();
            return brand;

        }
        public bool DeleteBrand(int id)
        {
            var brand = dbContext.Brands.Find(id);
            dbContext.Remove(brand);
            return dbContext.SaveChanges() > 0 ? true : false;
        }

        public ICollection<Brand> GetAllBrandsOfAVehicleType(int id)
        {
            return dbContext.Brands.Where(brands => brands.VehicleTypeId == id).ToList();
        }

        public ICollection<Brand> GetAllBrands()
        {
            return dbContext.Brands.ToList();
        }

        public async Task<bool> UpdateBrand(int id, Brand brand)
        {
            dbContext.Brands.Entry(brand).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            return true;

        }


        public Brand GetBrandById(int id)
        {
            return dbContext.Brands.Find(id);
        }

        public bool IsExists(int id)
        {
            return dbContext.Brands.Any(brand => brand.BrandId == id);
        }

    }
}