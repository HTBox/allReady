using System.ComponentModel.DataAnnotations;

namespace AllReady.ViewModels.Requests
{
    public class RequestApiViewModel
    {
        [Required]
        public string ProviderRequestId { get; set; } // incoming Red Cross "serial" field is mapped to this field, which will represnt the unique id of the request from an external provider

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
        public string Zip { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid Mobile phone Number")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        //AllReady's Requeset-specific non-required fields
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        //Red Cross specific fields

        [Required]
        public string Status { get; set; } //we only accept "new" request status from red cross
        
        //an example of an rc_region value from RC is IDMT for "rc_idaho_montana"
        public string ProviderData { get; set; } //incoming Red Cross "assigned_rc_region" field is mapped to this field. This field contains a red cross region and that region's zip code
    }
}