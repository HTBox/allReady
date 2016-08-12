using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Organization
{
    public class OrganizationDetailViewModel : IPrimaryContactViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Logo URL")]
        public string LogoUrl { get; set; }
        [Display(Name = "Website URL")]
        public string WebUrl { get; set; }

        /// <summary>
        /// Collection of the campaigns directly managed by this Organization
        /// </summary>
        public List<AllReady.Models.Campaign> Campaigns { get; set; }

        /// <summary>
        /// Application users which are members of this Organization.
        /// Users may be members of more than one organization.
        /// </summary>
        public List<ApplicationUser> Users { get; set; }

        [UIHint("Location")]
        public LocationDisplayViewModel Location { get; set; }

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
