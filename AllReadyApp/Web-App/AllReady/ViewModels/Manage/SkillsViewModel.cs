using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;
using Microsoft.AspNetCore.Identity;

namespace AllReady.ViewModels.Manage
{
    public class SkillsViewModel
    {
        [Display(Name = "My skills")]
        public List<UserSkill> AssociatedSkills { get; set; } = new List<UserSkill>();
    }
}