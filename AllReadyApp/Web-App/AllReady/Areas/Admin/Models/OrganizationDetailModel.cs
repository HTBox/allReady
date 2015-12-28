using AllReady.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class OrganizationDetailModel : IPrimaryContactModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string WebUrl { get; set; }

        /// <summary>
        /// Collection of the campaigns directly managed by this Organization
        /// </summary>
        public List<Campaign> Campaigns { get; set; }

        /// <summary>
        /// Application users which are members of this Organization.
        /// Users may be members of more than one organization.
        /// </summary>
        public List<ApplicationUser> Users { get; set; }

        [UIHint("Location")]
        public LocationDisplayModel Location { get; set; }

        [Display(Name = "First Name")]
        public string PrimaryContactFirstName { get; set; }

        [Display(Name = "Last Name")]
        public string PrimaryContactLastName { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        public string PrimaryContactPhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string PrimaryContactEmail { get; set; }
    }
}
