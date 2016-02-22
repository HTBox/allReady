using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class LocationEditModel
    {
        public int? Id { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        public string Name { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public string Country { get; set; } = "USA";
    }
}
