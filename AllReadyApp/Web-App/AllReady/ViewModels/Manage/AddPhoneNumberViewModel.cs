using System.ComponentModel.DataAnnotations;

namespace AllReady.ViewModels.Manage
{
    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Mobile phone number")]
        public string PhoneNumber { get; set; }
    }
}