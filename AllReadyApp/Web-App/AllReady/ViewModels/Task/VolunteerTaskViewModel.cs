using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels.Event;

namespace AllReady.ViewModels.Task
{
    public class VolunteerTaskViewModel
    {
        public VolunteerTaskViewModel()
        {
        }

        public VolunteerTaskViewModel(VolunteerTask task, string userId = null)
        {
            Id = task.Id;
            Name = task.Name;
            Description = task.Description;
            StartDateTime = task.StartDateTime;
            EndDateTime = task.EndDateTime;

            if (task.Event != null)
            {
                EventId = task.Event.Id;
                EventName = task.Event.Name;
            }

            if (task.Event?.Campaign != null)
            {
                CampaignId = task.Event.Campaign.Id;
                CampaignName = task.Event.Campaign.Name;
            }

            if (task.Organization != null)
            {
                OrganizationId = task.Organization.Id;
                OrganizationName = task.Organization.Name;
            }

            IsUserSignedUpForVolunteerTask = false;

            if (task.AssignedVolunteers != null)
            {
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    IsUserSignedUpForVolunteerTask = task.AssignedVolunteers.Any(au => au.User.Id == userId);
                }

                AssignedVolunteers = new List<VolunteerTaskSignupViewModel>();

                if (IsUserSignedUpForVolunteerTask)
                {
                    foreach (var t in task.AssignedVolunteers.Where(au => au.User.Id == userId))
                    {
                        AssignedVolunteers.Add(new VolunteerTaskSignupViewModel(t));
                    }
                }
            }

            if (task.RequiredSkills != null)
            {
                RequiredSkills = task.RequiredSkills.Select(t => t.SkillId);
                RequiredSkillObjects = task.RequiredSkills?.Select(t => t.Skill).Select(s => new SkillViewModel(s)).ToList();
            }

            NumberOfVolunteersRequired = task.NumberOfVolunteersRequired;
            NumberOfUsersSignedUp = task.NumberOfUsersSignedUp;
            IsLimitVolunteers = task.IsLimitVolunteers;
            IsAllowWaitList = task.IsAllowWaitList;
            IsClosed = task.IsClosed;
        }

        public int Id { get; set; }
        public bool IsNew { get; set; }

        [Display(Name = "Task Name")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Task Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Event")]
        public int EventId { get; set; }

        [Display(Name = "Event")]
        public string EventName { get; set; }

        public int CampaignId { get; set; }

        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }

        public int OrganizationId { get; set; }
        public int NumberOfVolunteersRequired { get; set; }
        public string OrganizationName { get; set; }

        [Display(Name = "Required Skills")]
        public IEnumerable<int> RequiredSkills { get; set; } = new List<int>();
        public List<SkillViewModel> RequiredSkillObjects { get; set; } = new List<SkillViewModel>();

        [Display(Name = "Starting time")]
        public DateTimeOffset StartDateTime { get; set; }
        [Display(Name = "Ending time")]
        public DateTimeOffset EndDateTime { get; set; }

        public bool IsUserSignedUpForVolunteerTask { get; }

        public List<VolunteerTaskSignupViewModel> AssignedVolunteers { get; set; } = new List<VolunteerTaskSignupViewModel>();

        public bool IsClosed { get; private set; }
     
        public int AcceptedVolunteerCount => AssignedVolunteers?.Where(v => v.Status == "Accepted").Count() ?? 0;
        public bool IsLimitVolunteers { get; set; } = true;
        public bool IsAllowWaitList { get; set; }
        public int NumberOfUsersSignedUp { get; set; }
        public bool IsFull => NumberOfUsersSignedUp >= NumberOfVolunteersRequired;
        public int AmountOfVolunteersNeeded => NumberOfVolunteersRequired - NumberOfUsersSignedUp;

        public string VolunteersRequiredText
        {
            get
            {
                if (IsFull) return "No more volunteers currently required";

                var pluralisation = AmountOfVolunteersNeeded == 1 ? " volunteer " : " volunteers ";

                return string.Concat(AmountOfVolunteersNeeded, pluralisation, "still needed");
            }
        }

        public bool IsAllowSignups => !IsLimitVolunteers || !IsFull || IsAllowWaitList;
        public string DisplayDateTime => GetDisplayDate();
        public string VolunteerLimitDisplay => $"Volunteer Limit: {NumberOfVolunteersRequired}, Spots Remaining: {NumberOfVolunteersRequired - NumberOfUsersSignedUp}";

        private string GetDisplayDate()
        {
            if (StartDateTime.Date == EndDateTime.Date)
            {
                return $"{StartDateTime:dddd, MMMM dd, yyyy} from {StartDateTime:t} - {EndDateTime:t}";
            }
            
            return $"{StartDateTime:dddd, MMMM dd, yyyy} at {StartDateTime:t} to {EndDateTime:dddd, MMMM dd, yyyy} at {EndDateTime:t}";
        }

        public VolunteerTaskViewModel(VolunteerTask task, bool isUserSignedupForTask) : this(task)
        {
            IsUserSignedUpForVolunteerTask = isUserSignedupForTask;
        }
    }
}