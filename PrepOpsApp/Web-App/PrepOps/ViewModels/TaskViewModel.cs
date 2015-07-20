using PrepOps.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PrepOps.ViewModels
{
    public class TaskViewModel
    {
        public TaskViewModel()
        {
        }

        public TaskViewModel(PrepOpsTask task)
        {
            Id = task.Id;
            Name = task.Name;
            Description = task.Description;


            if (task.StartDateTimeUtc.HasValue)
            {
                DateTime startDateWithUtcKind = DateTime.SpecifyKind(
                    DateTime.Parse(task.StartDateTimeUtc.Value.ToString()),
                    DateTimeKind.Utc);
                StartDateTime = new DateTimeOffset(startDateWithUtcKind);
            }

            if (task.EndDateTimeUtc.HasValue)
            {
                DateTime endDateWithUtcKind = DateTime.SpecifyKind(
                    DateTime.Parse(task.EndDateTimeUtc.Value.ToString()),
                    DateTimeKind.Utc);
                EndDateTime = new DateTimeOffset(endDateWithUtcKind);
            }

            if (task.Activity != null)
            {
                ActivityId = task.Activity.Id;
                ActivityName = task.Activity.Name;
            }

            if (task.Activity != null && task.Activity.Campaign != null)
            {
                CampaignId = task.Activity.Campaign.Id;
                CampaignName = task.Activity.Campaign.Name;
            }

            if (task.Tenant != null)
            {
                TenantId = task.Tenant.Id;
                TenantName = task.Tenant.Name;
            }

            IsUserSignedUpForTask = false;

            if (task.AssignedVolunteers != null)
            {
                this.AssignedVolunteers = new List<TaskSignupViewModel>();
                foreach (var t in task.AssignedVolunteers)
                {
                    this.AssignedVolunteers.Add(new TaskSignupViewModel(t));
                }
            }

        }

        public int Id { get; set; }
        [Display(Name = "Task name")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Task description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Activity")]
        public int ActivityId { get; set; }

        public string ActivityName { get; set; }

        public int CampaignId { get; set; }
        public string CampaignName { get; set; }

        public int TenantId { get; set; }
        public string TenantName { get; set; }

        [Display(Name = "Task starting time")]
        public DateTimeOffset? StartDateTime { get; set; }
        [Display(Name = "Task ending time")]
        public DateTimeOffset? EndDateTime { get; set; }

        public bool IsUserSignedUpForTask { get; private set; }

        public List<TaskSignupViewModel> AssignedVolunteers { get; set; }

        public TaskViewModel(PrepOpsTask task, bool isUserSignedupForTask)
            : this(task)
        {
            IsUserSignedUpForTask = isUserSignedupForTask;
        }

    }

    public static class TaskViewModelExtensions
    {
        public static TaskViewModel ToViewModel(this PrepOpsTask task)
        {
            return new TaskViewModel(task);
        }

        public static IEnumerable<TaskViewModel> ToViewModel(this IEnumerable<PrepOpsTask> tasks)
        {
            return tasks.Select(task => task.ToViewModel());
        }

        public static PrepOpsTask ToModel(this TaskViewModel task, IPrepOpsDataAccess dataAccess)
        {
            var activity = dataAccess.GetActivity(task.ActivityId);

            if (activity == null)
                return null;

            bool newTask = true;
            PrepOpsTask dbtask;

            if (task.Id == 0)
            {
                dbtask = new PrepOpsTask();
            }
            else
            {
                dbtask = dataAccess.GetTask(task.Id);
                newTask = false;
            }

            // dbtask.Id = task.Id;
            dbtask.Description = task.Description;
            dbtask.Activity = activity;
            dbtask.EndDateTimeUtc = task.EndDateTime.HasValue ? task.EndDateTime.Value.UtcDateTime : new Nullable<DateTime>();
            dbtask.StartDateTimeUtc = task.EndDateTime.HasValue ? task.StartDateTime.Value.UtcDateTime : new Nullable<DateTime>();
            dbtask.Name = task.Name;

            // Workaround:  POST is bringing in empty AssignedVolunteers.  Clean this up. Discussing w/ Kiran Challa.
            // Workaround: the if statement is superflous, and should go away once we have the proper fix referenced above.
            if (task.AssignedVolunteers != null)
            {
                var bogusAssignedVolunteers = (from assignedVolunteer in task.AssignedVolunteers
                                               where string.IsNullOrEmpty(assignedVolunteer.UserId)
                                               select assignedVolunteer).ToList();
                foreach (var bogus in bogusAssignedVolunteers)
                {
                    task.AssignedVolunteers.Remove(bogus);
                }
            }
            // end workaround


            if (task.AssignedVolunteers != null && task.AssignedVolunteers.Count > 0)
            {
                var taskUsersList = task.AssignedVolunteers.Select(tvm => new TaskUsers
                {
                    Task = dbtask,
                    User = dataAccess.GetUser(tvm.UserId)
                }).ToList();

                // We may be updating an existing task
                if (newTask || dbtask.AssignedVolunteers.Count == 0)
                {
                    dbtask.AssignedVolunteers = taskUsersList;
                }
                else
                {
                    // Can probably rewrite this more efficiently.
                    foreach (TaskUsers taskUsers in taskUsersList)
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

        public static IEnumerable<PrepOpsTask> ToModel(this IEnumerable<TaskViewModel> tasks, IPrepOpsDataAccess dataContext)
        {
            return tasks.Select(task => task.ToModel(dataContext));
        }

    }
}
