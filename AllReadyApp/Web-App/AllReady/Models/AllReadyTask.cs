using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{
    public class AllReadyTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual Activity Activity { get; set; }
        public DateTime? StartDateTimeUtc { get; set; }
        public DateTime? EndDateTimeUtc { get; set; }
        public virtual List<TaskUsers> AssignedVolunteers { get; set; }
        public virtual List<TaskSkill> RequiredSkills { get; set; }
    }
}