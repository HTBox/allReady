using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Shared
{
    public class LocationEditViewModel
    {
        public int? Id { get; set; }
        [Required]
        [Display(Name = "Address Line 1")]
        public string Address1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Display(Name = "Postal Code")]
        [Required]
        public string PostalCode { get; set; }
        public string Name { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string Country { get; set; } = "USA";
    }
}
