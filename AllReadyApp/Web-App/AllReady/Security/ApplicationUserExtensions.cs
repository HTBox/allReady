using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels.Manage;
using Microsoft.AspNetCore.Identity;

namespace AllReady.Security
{
    public static class ApplicationUserExtensions
    {
        public static bool IsUserType(this ApplicationUser user, UserType userType)
        {
            return user?.Claims != null && user.Claims.Any(c => c.ClaimType == ClaimTypes.UserType && c.ClaimValue == Enum.GetName(typeof(UserType), userType));
        }

        public static int? GetOrganizationId(this ApplicationUser user)
        {
            int? result = null;
            var organizationIdClaim = user.Claims.FirstOrDefault(c => c.ClaimType == ClaimTypes.Organization);
            if (organizationIdClaim != null)
            {
                int organizationId;
                if (int.TryParse(organizationIdClaim.ClaimValue, out organizationId))
                    result = organizationId;
            }

            return result;
        }

        public static async Task<IndexViewModel> ToViewModel(
            this ApplicationUser user, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager)
        {
            var skills = new SkillsViewModel
            {
                AssociatedSkills = user.AssociatedSkills
            };

            var profileCompletenessWarnings = user.ValidateProfileCompleteness();
            var profile = new ProfileViewModel
            {
                HasPassword = await userManager.HasPasswordAsync(user),
                EmailAddress = user.Email,
                IsEmailAddressConfirmed = user.EmailConfirmed,
                IsPhoneNumberConfirmed = user.PhoneNumberConfirmed,
                PhoneNumber = await userManager.GetPhoneNumberAsync(user),
                TwoFactor = await userManager.GetTwoFactorEnabledAsync(user),
                Logins = await userManager.GetLoginsAsync(user),
                BrowserRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user),
                TimeZoneId = user.TimeZoneId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProposedNewEmailAddress = user.PendingNewEmail,
                IsProfileComplete = user.IsProfileComplete(),
                ProfileCompletenessWarnings = profileCompletenessWarnings.Select(p => p.ErrorMessage)
            };

            return new IndexViewModel
            {
                ProfileViewModel = profile,
                SkillsViewModel = skills,
            };
        }
    }
}