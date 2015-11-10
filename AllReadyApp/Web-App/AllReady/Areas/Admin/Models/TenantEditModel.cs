using AllReady.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class TenantEditModel
    {
        
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
        public string LogoUrl { get; set; }
        public string WebUrl { get; set; }
          
        public LocationEditModel Location { get; set; }

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
