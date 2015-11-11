using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Models
{
    public interface IPrimaryContactModel
    {
        [Display(Name = "First Name")]
        string PrimaryContactFirstName { get; set; }

        [Display(Name = "Last Name")]
        string PrimaryContactLastName { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        string PrimaryContactPhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        string PrimaryContactEmail { get; set; }

    }
}
