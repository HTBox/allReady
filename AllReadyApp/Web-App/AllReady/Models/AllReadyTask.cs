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
        public DateTimeOffset? StartDateTimeUtc { get; set; }
        public DateTimeOffset? EndDateTimeUtc { get; set; }
        public List<TaskSignup> AssignedVolunteers { get; set; } = new List<TaskSignup>();
        public List<TaskSkill> RequiredSkills { get; set; } = new List<TaskSkill>();
    }
}