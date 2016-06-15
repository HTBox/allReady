using System.ComponentModel.DataAnnotations;

namespace AllReady.ViewModels.Account
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Display(Name="Phone number")]
        public string PhoneNumber { get; set; }
    }
}