using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AllReady.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Associated skills")]
        public List<UserSkill> AssociatedSkills { get; set; } = new List<UserSkill>();

        public string Name { get; set; }

        [Display(Name = "Time Zone")]
        [Required]
        public string TimeZoneId { get; set; }

        public string PendingNewEmail { get; set; }

        public IEnumerable<ValidationResult> ValidateProfileCompleteness()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            if (!EmailConfirmed)
            {
                validationResults.Add(new ValidationResult("Verify your email address", new string[] { nameof(Email) }));
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                validationResults.Add(new ValidationResult("Enter your name", new string[] { nameof(Name) }));
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                validationResults.Add(new ValidationResult("Add a phone number", new string[] { nameof(PhoneNumber) }));
            }
            else if (!PhoneNumberConfirmed)
            {
                validationResults.Add(new ValidationResult("Confirm your phone number", new string[] { nameof(PhoneNumberConfirmed) }));
            }
            return validationResults;
        }

        public bool IsProfileComplete()
        {
            return !ValidateProfileCompleteness().Any();
        }
    }
}