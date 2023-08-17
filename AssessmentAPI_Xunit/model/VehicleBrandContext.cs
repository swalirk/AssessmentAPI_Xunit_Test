using Microsoft.EntityFrameworkCore;

namespace AssessmentAPI_Xunit.model
{
    public class VehicleBrandContext:DbContext
    {
       public VehicleBrandContext(DbContextOptions<VehicleBrandContext> options) : base(options)
        {

        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }

       
    }
}
