using System.ComponentModel.DataAnnotations;
using AllReady.Areas.Admin.ViewModels.Shared;

namespace AllReady.Areas.Admin.ViewModels.Organization
{
    public class OrganizationEditViewModel : IPrimaryContactViewModel
    {        
        public int Id { get; set; }

        [Required]
        [Display(Name = "Organization Name")]
        public string Name { get; set; }

        [Display(Name = "Logo URL")]
        public string LogoUrl { get; set; }

        [Display(Name = "Website URL")]
        public string WebUrl { get; set; }
          
        [UIHint("Location")]
        public LocationEditViewModel Location { get; set; }

        [Display(Name = "First Name")]
        public string PrimaryContactFirstName { get; set; }

        [Display(Name = "Last Name")]
        public string PrimaryContactLastName { get; set; }

        [Display(Name = "Mobile phone Number")]
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
        [Display(Name = "Privacy Policy URL")]
        public string PrivacyPolicyUrl { get; set; }

        [Display(Name = "Privacy Policy")]
        public string PrivacyPolicy { get; set; }
    }
}
