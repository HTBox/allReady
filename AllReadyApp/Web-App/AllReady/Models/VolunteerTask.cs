﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{
    public class VolunteerTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual Organization Organization { get; set; }

        public int EventId { get; set; }
        public virtual Event Event { get; set; }

        public int NumberOfVolunteersRequired { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public List<VolunteerTaskSignup> AssignedVolunteers { get; set; } = new List<VolunteerTaskSignup>();
        public List<TaskSkill> RequiredSkills { get; set; } = new List<TaskSkill>();
        public bool IsLimitVolunteers { get; set; } = true;
        public bool IsAllowWaitList { get; set; }

        [NotMapped]
        public int NumberOfUsersSignedUp => AssignedVolunteers.Count;
        [NotMapped]
        public bool IsFull => NumberOfUsersSignedUp >= NumberOfVolunteersRequired;
        [NotMapped]
        public bool IsAllowSignups => !IsLimitVolunteers || !IsFull || IsAllowWaitList;
        [NotMapped]
        public bool IsClosed => EndDateTime.UtcDateTime < DateTimeOffset.UtcNow;
    }
}