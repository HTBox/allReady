using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
    public class UserSkill
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

    }
}
