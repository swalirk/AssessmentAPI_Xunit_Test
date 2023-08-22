using AssessmentAPI_Xunit.model;
using AssessmentAPI_Xunit.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace AssessmentAPI_Xunit.Service
{
    public class VehicleRepository : IVehicleInterface
    {
        private readonly VehicleBrandContext dbContext;

        public VehicleRepository(VehicleBrandContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<VehicleType> AddVehicleType(VehicleType vehicle)
        {
            await dbContext.VehicleTypes.AddAsync(vehicle);

            await dbContext.SaveChangesAsync();
            return vehicle;
        }

        public async Task<bool> UpdateVehicleType(int id, VehicleType vehicletype)
        {
            dbContext.VehicleTypes.Entry(vehicletype).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            return true;
        }
        


        public ICollection<VehicleType> GetAllVehicleTypes()
        {
            return dbContext.VehicleTypes.ToList();

        }

        public bool IsExists(int id)
        {
            return dbContext.VehicleTypes.Any(vehicleType => vehicleType.VehicleTypeId == id);
        }

        public VehicleType GetVehicleTypeById(int id)
        {
            return dbContext.VehicleTypes.FirstOrDefault(vt => vt.VehicleTypeId == id);
        }
    }
}