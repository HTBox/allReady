using System.ComponentModel.DataAnnotations;

namespace AllReady.ViewModels.Manage
{
    public class VerifyPhoneNumberViewModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Mobile phone number")]
        public string PhoneNumber { get; set; }
    }
}