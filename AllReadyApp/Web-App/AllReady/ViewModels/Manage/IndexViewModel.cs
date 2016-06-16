using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AllReady.Models;
using Microsoft.AspNet.Identity;
using System.Linq;

namespace AllReady.ViewModels.Manage
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            Logins = new List<UserLoginInfo>();
            AssociatedSkills = new List<UserSkill>();
        }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

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

        public bool IsProfileComplete { get; set; }

        public IEnumerable<string> ProfileCompletenessWarnings { get; set; }
    }

    public static class IndexViewModelExtensions
    {
        public static async Task<IndexViewModel> ToViewModel(this ApplicationUser user, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            var profileCompletenessWarnings = user.ValidateProfileCompleteness();
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
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProposedNewEmailAddress = user.PendingNewEmail,
                IsProfileComplete = user.IsProfileComplete(),
                ProfileCompletenessWarnings = profileCompletenessWarnings.Select(p => p.ErrorMessage)
            };
            return result;
        }
    }
}