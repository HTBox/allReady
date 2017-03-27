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
        public string PostalCode { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid Mobile phone Number")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        //Red Cross specific fields
        [Required]
        public string Status { get; set; } //we only accept "new" request status from red cross
        public string ProviderData { get; set; } //incoming Red Cross "assigned_rc_region" field is mapped to this field. An example of an assigned_rc_region value from RC is IDMT for "rc_idaho_montana"
    }
}