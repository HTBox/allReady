﻿using System.ComponentModel.DataAnnotations;

namespace AllReady.ViewModels.Account
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name="Mobile phone Number")]
        public string PhoneNumber { get; set; }

        public string ReturnUrl { get; set; }

        public string LoginProvider { get; set; }

        public bool EmailIsVerifiedByExternalLoginProvider { get; set; }
    }
}