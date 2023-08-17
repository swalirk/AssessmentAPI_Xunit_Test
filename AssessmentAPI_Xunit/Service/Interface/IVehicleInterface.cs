using AssessmentAPI_Xunit.model;

namespace AssessmentAPI_Xunit.Service.Interface
{
    public interface IVehicleInterface
    {
        public Task<VehicleType> AddVehicleType(VehicleType vehicleType);
        public ICollection<VehicleType> GetAllVehicleTypes();
        public Task<bool> UpdateVehicleType(int id, VehicleType vehicletype, VehicleType existingtype);
        public bool IsExists(int id);

        public VehicleType GetVehicleTypeById(int id);
    }
}
