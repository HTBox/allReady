using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace AllReady.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Associated skills")]
        public List<UserSkill> AssociatedSkills { get; set; } = new List<UserSkill>();

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [NotMapped]
        public string Name => $"{FirstName} {LastName}";

        [Display(Name = "Time Zone")]
        [Required]
        public string TimeZoneId { get; set; }

        public string PendingNewEmail { get; set; }

        /// <summary>
        /// Navigation to campaigns the user has access to manage
        /// </summary>
        public List<CampaignManager> ManagedCampaigns { get; set; }

        /// <summary>
        /// Navigation to events the user has access to manage
        /// </summary>
        public List<EventManager> ManagedEvents { get; set; }

        /// <summary>
        /// Navigation to event invites the user has sent
        /// </summary>
        public List<EventManagerInvite> SentEventManagerInvites { get; set; }

        /// <summary>
        /// Navigation to campaign invites the user has sent
        /// </summary>
        public List<CampaignManagerInvite> SentCampaignManagerInvites { get; set; }
        
        public IEnumerable<ValidationResult> ValidateProfileCompleteness()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            if (!EmailConfirmed)
            {
                validationResults.Add(new ValidationResult("Verify your email address", new[] { nameof(Email) }));
            }

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                validationResults.Add(new ValidationResult("Enter your first name", new[] { nameof(FirstName) }));
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                validationResults.Add(new ValidationResult("Enter your last name", new[] { nameof(LastName) }));
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                validationResults.Add(new ValidationResult("Add a mobile phone number", new[] { nameof(PhoneNumber) }));
            }
            else if (!PhoneNumberConfirmed)
            {
                validationResults.Add(new ValidationResult("Confirm your mobile phone number", new[] { nameof(PhoneNumberConfirmed) }));
            }
            return validationResults;
        }

        public bool IsProfileComplete()
        {
            return !ValidateProfileCompleteness().Any();
        }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; } = new List<IdentityUserLogin<string>>();

        /// <summary>
        /// Navigation property to get all request comments for this user
        /// </summary>
        public virtual ICollection<RequestComment> RequestComments { get; set; } = new List<RequestComment>();
    }
}
