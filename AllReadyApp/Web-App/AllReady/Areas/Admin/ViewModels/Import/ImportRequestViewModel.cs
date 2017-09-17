using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Import
{
    public class ImportRequestViewModel
    {
        //maps to our Request.ProviderRequestId field
        [Required]
        public string Id { get; set; }

        //AllReady's Request-specific required fields
        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid Mobile phone Number")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string ProviderData { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}