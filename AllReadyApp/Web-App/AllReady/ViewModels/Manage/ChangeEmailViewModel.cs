using System.ComponentModel.DataAnnotations;

namespace AllReady.ViewModels.Manage
{
    /// <summary>
    /// Models the information required for a change email request
    /// </summary>
    public class ChangeEmailViewModel
    {
        /// <summary>
        /// The current password for the user
        /// </summary>
        /// <remarks>We collect this since changing the primary email is a sensitive action</remarks>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string Password { get; set; }

        /// <summary>
        /// The proposed new email address from the user
        /// </summary>
        [Required]
        [Display(Name = "New email address")]
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}
