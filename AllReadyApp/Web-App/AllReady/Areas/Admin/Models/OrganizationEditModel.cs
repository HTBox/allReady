using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class OrganizationEditModel : IPrimaryContactModel
    {        
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }        

        public string LogoUrl { get; set; }

        [Display(Name = "Website URL")]
        public string WebUrl { get; set; }
          
        [UIHint("Location")]
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

        [Display(Name = "Full Description")]
        public string Description { get; set; }

        [Display(Name = "Summary Description")]
        [MaxLength(250)]
        public string Summary { get; set; }

        [Url]
        [Display(Name = "Privacy Policy Url")]
        public string PrivacyPolicyUrl { get; set; }

        [Display(Name = "Privacy Policy")]
        public string PrivacyPolicy { get; set; }
    }
}
