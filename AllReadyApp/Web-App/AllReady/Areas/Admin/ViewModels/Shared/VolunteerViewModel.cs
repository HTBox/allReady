using AllReady.Models;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.ViewModels.Shared
{
    public class VolunteerViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool HasVolunteered { get; set; }
        public string Status { get; set; }
        public string AdditionalInfo { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public List<UserSkill> AssociatedSkills { get; set; }
    }
}
