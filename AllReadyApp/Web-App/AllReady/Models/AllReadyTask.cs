using System;
using System.Collections.Generic;

namespace AllReady.Models
{
    public class AllReadyTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Activity Activity { get; set; }
        public int NumberOfVolunteersRequired { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }
        public List<TaskSignup> AssignedVolunteers { get; set; } = new List<TaskSignup>();
        public List<TaskSkill> RequiredSkills { get; set; } = new List<TaskSkill>();
    }
}