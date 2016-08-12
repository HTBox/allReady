using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Organization
{
    public interface IPrimaryContactModel
    {
        [Display(Name = "First Name")]
        string PrimaryContactFirstName { get; set; }

        [Display(Name = "Last Name")]
        string PrimaryContactLastName { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        string PrimaryContactPhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        string PrimaryContactEmail { get; set; }

    }
}
