using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class ContactInformationModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        public LocationDisplayModel Location {get;set;}
    }
}
