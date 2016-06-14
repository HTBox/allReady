using System.ComponentModel.DataAnnotations;

namespace AllReady.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}