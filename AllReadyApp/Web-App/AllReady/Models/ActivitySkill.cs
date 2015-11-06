using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
    public class ActivitySkill
    {
        public int ActivityId { get; set; }
        public Activity Activity { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

    }
}
