using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Mobile phone Number")]
        public string PhoneNumber { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }

        public List<OrganizationContact> OrganizationContacts { get; set; }

        public List<CampaignContact> CampaignContacts { get; set; }
    }
}
