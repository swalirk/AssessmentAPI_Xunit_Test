using AssessmentAPI_Xunit.model;

namespace AssessmentAPI_Xunit.Service.Interface
{
    public interface IBrandInteface
    {
        public Task<Brand> AddBrand(Brand brand);

        public bool DeleteBrand(int id);
        public bool IsExists(int id);

        public ICollection<Brand> GetAllBrandsOfAVehicleType(int id);

        public ICollection<Brand> GetAllBrands();

        public Task<bool> UpdateBrand(int id, Brand brand, Brand existingBrand);
        public Brand GetBrandById(int id);

    }
}