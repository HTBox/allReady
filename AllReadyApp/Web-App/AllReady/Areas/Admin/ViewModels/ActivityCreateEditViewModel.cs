using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels
{
    public class ActivityCreateEditViewModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Display(Name = "Start date")]
        public DateTime StartDateTime { get; set; }

        [Required]
        [Display(Name = "End date")]
        public DateTime EndDateTime { get; set; }

        [Display(Name = "Required skills")]
        public List<ActivitySkill> RequiredSkills { get; set; } = new List<ActivitySkill>();
    }
}
