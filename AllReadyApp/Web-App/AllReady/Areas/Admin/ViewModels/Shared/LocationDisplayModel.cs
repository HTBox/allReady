using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class LocationDisplayModel
    {
        public int Id { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Name { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public string Country { get; set; } = "USA";
    }
}
