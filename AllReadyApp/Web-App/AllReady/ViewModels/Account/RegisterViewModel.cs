using System.ComponentModel.DataAnnotations;

namespace AllReady.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [RegularExpression(@"^[\wáâàãçéêíóôõúüÁÂÀÃÇÉÊÍÓÔÕÚÜ\-\'\.\, ]+$", ErrorMessage = "The {0} should not contain any special characters.")]
        [Display(Name = "First Name")]
        public string FirstName{ get; set; }

        [Required]
        [RegularExpression(@"^[\wáâàãçéêíóôõúüÁÂÀÃÇÉÊÍÓÔÕÚÜ\-\'\.\, ]+$", ErrorMessage = "The {0} should not contain any special characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Mobile phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
