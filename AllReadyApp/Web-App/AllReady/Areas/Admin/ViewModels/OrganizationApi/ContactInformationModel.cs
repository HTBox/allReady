using System.ComponentModel.DataAnnotations;
using AllReady.Areas.Admin.ViewModels.Shared;

namespace AllReady.Areas.Admin.ViewModels.OrganizationApi
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
