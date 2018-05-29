using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;
using Microsoft.AspNetCore.Identity;

namespace AllReady.ViewModels.Manage
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        [MaxLength(200, ErrorMessage = "Name cannot be longer than 200 characters.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(200, ErrorMessage = "Name cannot be longer than 200 characters.")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        public bool IsEmailAddressConfirmed { get; set; }

        public bool IsPhoneNumberConfirmed { get; set; }

        public bool HasPassword { get; set; }

        public IList<UserLoginInfo> Logins { get; set; } = new List<UserLoginInfo>();

        public string PhoneNumber { get; set; }

        public bool TwoFactor { get; set; }

        public bool BrowserRemembered { get; set; }

        [Required]
        [Display(Name = "Time Zone")]
        public string TimeZoneId { get; set; }

        public string ProposedNewEmailAddress { get; set; }

        public bool IsProfileComplete { get; set; }

        public IEnumerable<string> ProfileCompletenessWarnings { get; set; }
    }
}