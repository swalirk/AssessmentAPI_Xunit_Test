using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AssessmentAPI_Xunit.model
{
    public class Brand
    {
        public int BrandId { get; set; }

        [ForeignKey("VehicleType")]
        public int VehicleTypeId { get; set; }
        public string? BrandName { get; set; }
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }

        [JsonIgnore]
        public virtual VehicleType? VehicleType { get; }
    }
}
