using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.VolunteerTask
{
    public class RemoveVolunteerViewModel
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public bool Notify { get; set; }

    }
}
