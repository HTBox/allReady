using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
                validationResults.Add(new ValidationResult("Add a phone number", new[] { nameof(PhoneNumber) }));
            }
            else if (!PhoneNumberConfirmed)
            {
                validationResults.Add(new ValidationResult("Confirm your phone number", new[] { nameof(PhoneNumberConfirmed) }));
            }
            return validationResults;
        }

        public bool IsProfileComplete()
        {
            return !ValidateProfileCompleteness().Any();
        }
    }
}