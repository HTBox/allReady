using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AllReady.ViewModels;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc.Rendering;

namespace AllReady.Models
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            Logins = new List<UserLoginInfo>();
            AssociatedSkills = new List<UserSkill>();
        }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        public bool IsEmailAddressConfirmed { get; set; }

        public bool IsPhoneNumberConfirmed { get; set; }

        public bool HasPassword { get; set; }

        public IList<UserLoginInfo> Logins { get; set; }

        public string PhoneNumber { get; set; }

        public bool TwoFactor { get; set; }

        public bool BrowserRemembered { get; set; }

        [Display(Name = "My skills")]
        public List<UserSkill> AssociatedSkills { get; set; }

        [Display(Name = "Time Zone")]
        [Required]
        public string TimeZoneId { get; set; }

        public string ProposedNewEmailAddress { get; set; }
    }

    public static class IndexViewModelExtensions
    {
        public static async Task<IndexViewModel> ToViewModel(this ApplicationUser user, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            var result = new IndexViewModel
            {
                HasPassword = await userManager.HasPasswordAsync(user),
                EmailAddress = user.Email,
                IsEmailAddressConfirmed = user.EmailConfirmed,
                IsPhoneNumberConfirmed = user.PhoneNumberConfirmed,
                PhoneNumber = await userManager.GetPhoneNumberAsync(user),
                TwoFactor = await userManager.GetTwoFactorEnabledAsync(user),
                Logins = await userManager.GetLoginsAsync(user),
                BrowserRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user),
                AssociatedSkills = user.AssociatedSkills,
                TimeZoneId = user.TimeZoneId,
                Name = user.Name,
                ProposedNewEmailAddress = user.PendingNewEmail
            };
            return result;
        }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

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
        public string NewEmail { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }
    }
}
