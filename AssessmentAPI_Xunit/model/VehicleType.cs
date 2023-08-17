
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AssessmentAPI_Xunit.model
{
    public class VehicleType
    {
        public VehicleType()
        {
            this.Brands = new HashSet<Brand>();
        }
        public int VehicleTypeId { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }

        [JsonIgnore]
        public virtual ICollection<Brand>? Brands { get; }
    }
}
