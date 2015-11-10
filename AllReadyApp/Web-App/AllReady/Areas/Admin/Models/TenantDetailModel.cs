using AllReady.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Models
{
    public class TenantDetailModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string WebUrl { get; set; }

        /// <summary>
        /// Collection of the campaigns directly managed by this Tenant
        /// </summary>
        public List<Campaign> Campaigns { get; set; }

        /// <summary>
        /// Application users which are members of this Tenant.
        /// Users may be members of more than one tenant.
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
