using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
    public class TaskSkill
    {
        public int TaskId { get; set; }
        public AllReadyTask Task { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

    }
}
