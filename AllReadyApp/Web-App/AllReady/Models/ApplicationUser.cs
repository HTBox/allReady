using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AllReady.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Associated skills")]
        public List<UserSkill> AssociatedSkills { get; set; } = new List<UserSkill>();

        //TODO: come back and make this a not mapped field so reads do not have to concatentate when needing full name
        //[NotMapped]
        //public string Name { get; set; }

        //TODO: should we allow the creation of an ApplicationUser w/out a first and last name?
        public string Forename { get; set; }

        public string Surname { get; set; }

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

            //if (string.IsNullOrWhiteSpace(Name))
            //{
            //    validationResults.Add(new ValidationResult("Enter your name", new string[] { nameof(Name) }));
            //}

            if (string.IsNullOrWhiteSpace(Forename))
            {
                validationResults.Add(new ValidationResult("Enter your first name", new[] { nameof(Forename) }));
            }

            if (string.IsNullOrWhiteSpace(Surname))
            {
                validationResults.Add(new ValidationResult("Enter your first name", new[] { nameof(Surname) }));
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