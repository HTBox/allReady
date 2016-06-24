using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AllReady.Features.Event;
using AllReady.Features.Manage;
using AllReady.Features.Tasks;
using AllReady.Models;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.ViewModels.Task
{
    public class TaskViewModel
    {
        public TaskViewModel()
        {
        }

        public TaskViewModel(AllReadyTask task, string userId = null)
        {
            Id = task.Id;
            Name = task.Name;
            Description = task.Description;

            if (task.StartDateTime.HasValue)
            {
                StartDateTime = task.StartDateTime.Value;
            }

            if (task.EndDateTime.HasValue)
            {
                EndDateTime = task.EndDateTime.Value;
            }

            if (task.Event != null)
            {
                EventId = task.Event.Id;
                eventName = task.Event.Name;
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

            IsUserSignedUpForTask = false;
            if (task.AssignedVolunteers != null)
            {
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    IsUserSignedUpForTask = task.AssignedVolunteers.Any(au => au.User.Id == userId);
                }

                AssignedVolunteers = new List<TaskSignupViewModel>();

                if (IsUserSignedUpForTask)
                {
                    foreach (var t in task.AssignedVolunteers.Where(au => au.User.Id == userId))
                    {
                        AssignedVolunteers.Add(new TaskSignupViewModel(t));
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
        public string eventName { get; set; }

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
        public DateTimeOffset? StartDateTime { get; set; }
        [Display(Name = "Ending time")]
        public DateTimeOffset? EndDateTime { get; set; }

        public bool IsUserSignedUpForTask { get; private set; }

        public List<TaskSignupViewModel> AssignedVolunteers { get; set; } = new List<TaskSignupViewModel>();

        public bool IsClosed { get; private set; } = false;
     
        public int AcceptedVolunteerCount => AssignedVolunteers?.Where(v => v.Status == "Accepted").Count() ?? 0;
        public bool IsLimitVolunteers { get; set; } = true;
        public bool IsAllowWaitList { get; set; } = true;
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
            if (StartDateTime == null) return "Dates not specified";
            if (EndDateTime == null) return $"{StartDateTime:dddd, MMMM dd, yyyy} at {StartDateTime:t}";
            if (StartDateTime.Value.Date == EndDateTime.Value.Date)
            {
                return $"{StartDateTime:dddd, MMMM dd, yyyy} from {StartDateTime:t} - {EndDateTime:t}";
            }
            return $"{StartDateTime:dddd, MMMM dd, yyyy} at {StartDateTime:t} to {EndDateTime:dddd, MMMM dd, yyyy} at {EndDateTime:t}";
        }

        public TaskViewModel(AllReadyTask task, bool isUserSignedupForTask)
            : this(task)
        {
            IsUserSignedUpForTask = isUserSignedupForTask;
        }
    }

    public static class TaskViewModelExtensions
    {
        public static TaskViewModel ToViewModel(this AllReadyTask task)
        {
            return new TaskViewModel(task);
        }

        public static IEnumerable<TaskViewModel> ToViewModel(this IEnumerable<AllReadyTask> tasks)
        {
            return tasks.Select(task => task.ToViewModel());
        }

        public static AllReadyTask ToModel(this TaskViewModel taskViewModel, IMediator mediator)
        {
            var campaignEvent = mediator.Send(new EventByIdQuery { EventId = taskViewModel.EventId });
            if (campaignEvent == null)
            {
                return null;
            }

            var newTask = true;
            AllReadyTask dbtask;
            if (taskViewModel.Id == 0)
            {
                dbtask = new AllReadyTask();
            }
            else
            {
                dbtask = mediator.Send(new TaskByTaskIdQuery { TaskId = taskViewModel.Id });
                newTask = false;
            }

            dbtask.Id = taskViewModel.Id;
            dbtask.Description = taskViewModel.Description;
            dbtask.Event = campaignEvent;
            dbtask.EndDateTime = taskViewModel.EndDateTime.HasValue ? taskViewModel.EndDateTime.Value.UtcDateTime : new DateTime?();
            dbtask.StartDateTime = taskViewModel.EndDateTime.HasValue ? taskViewModel.StartDateTime.Value.UtcDateTime : new DateTime?();
            dbtask.Name = taskViewModel.Name;
            dbtask.RequiredSkills = dbtask.RequiredSkills ?? new List<TaskSkill>();
            taskViewModel.RequiredSkills = taskViewModel.RequiredSkills ?? new List<int>();
            ////Remove old skills
            //dbtask.RequiredSkills.RemoveAll(ts => !taskViewModel.RequiredSkills.Any(s => ts.SkillId == s));
            ////Add new skills
            //dbtask.RequiredSkills.AddRange(taskViewModel.RequiredSkills
            //    .Where(rs => !dbtask.RequiredSkills.Any(ts => ts.SkillId == rs))
            //    .Select(rs => new TaskSkill() { SkillId = rs, TaskId = taskViewModel.Id }));

            // Workaround:  POST is bringing in empty AssignedVolunteers.  Clean this up. Discussing w/ Kiran Challa.
            // Workaround: the if statement is superflous, and should go away once we have the proper fix referenced above.
            if (taskViewModel.AssignedVolunteers != null)
            {
                var bogusAssignedVolunteers = (from assignedVolunteer in taskViewModel.AssignedVolunteers
                                               where string.IsNullOrEmpty(assignedVolunteer.UserId)
                                               select assignedVolunteer).ToList();
                foreach (var bogus in bogusAssignedVolunteers)
                {
                    taskViewModel.AssignedVolunteers.Remove(bogus);
                }
            }
            // end workaround

            if (taskViewModel.AssignedVolunteers != null && taskViewModel.AssignedVolunteers.Count > 0)
            {
                var taskUsersList = taskViewModel.AssignedVolunteers.Select(tvm => new TaskSignup
                {
                    Task = dbtask,
                    User = mediator.Send(new UserByUserIdQuery { UserId = tvm.UserId })
                }).ToList();

                // We may be updating an existing task
                if (newTask || dbtask.AssignedVolunteers.Count == 0)
                {
                    dbtask.AssignedVolunteers = taskUsersList;
                }
                else
                {
                    // Can probably rewrite this more efficiently.
                    foreach (var taskUsers in taskUsersList)
                    {
                        if (!(from entry in dbtask.AssignedVolunteers
                              where entry.User.Id == taskUsers.User.Id
                              select entry).Any())
                        {
                            dbtask.AssignedVolunteers.Add(taskUsers);
                        }
                    }
                }
            }

            return dbtask;
        }

        public static IEnumerable<AllReadyTask> ToModel(this IEnumerable<TaskViewModel> tasks, IMediator mediator)
        {
            return tasks.Select(task => task.ToModel(mediator));
        }
    }
}