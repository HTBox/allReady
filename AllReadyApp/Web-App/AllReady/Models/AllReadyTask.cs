using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{
    public class AllReadyTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Event Event { get; set; }
        public int NumberOfVolunteersRequired { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }
        public List<TaskSignup> AssignedVolunteers { get; set; } = new List<TaskSignup>();
        public List<TaskSkill> RequiredSkills { get; set; } = new List<TaskSkill>();
        public bool IsLimitVolunteers { get; set; } = true;
        public bool IsAllowWaitList { get; set; } = true;

        [NotMapped]
        public int NumberOfUsersSignedUp => AssignedVolunteers.Count;
        [NotMapped]
        public bool IsFull => NumberOfUsersSignedUp >= NumberOfVolunteersRequired;
        [NotMapped]
        public bool IsAllowSignups => !IsLimitVolunteers || !IsFull || IsAllowWaitList;

        [NotMapped]
        public bool IsClosed
        {
            get
            {
                if (EndDateTime.HasValue)
                {
                    return EndDateTime.Value.UtcDateTime < DateTimeOffset.UtcNow;
                }

                return false;
            }
        }
    }
}