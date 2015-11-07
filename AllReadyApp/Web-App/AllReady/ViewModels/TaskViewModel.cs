using AllReady.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AllReady.ViewModels
{
    public class TaskViewModel
    {
        public TaskViewModel()
        {
        }

        public TaskViewModel(AllReadyTask task)
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

            if (task.RequiredSkills != null)
            {
                this.RequiredSkills = task.RequiredSkills.Select(t => t.SkillId);
            }

        }

        public int Id { get; set; }
        public bool IsNew { get; set; }

        [Display(Name = "Task Name")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Task Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Activity")]
        public int ActivityId { get; set; }

        [Display(Name = "Activity")]
        public string ActivityName { get; set; }

        public int CampaignId { get; set; }

        [Display(Name="Campaign")]
        public string CampaignName { get; set; }

        public int TenantId { get; set; }
        public string TenantName { get; set; }

        [Display(Name = "Required Skills")]
        public IEnumerable<int> RequiredSkills { get; set; } = new List<int>();

        [Display(Name = "Starting time")]
        public DateTimeOffset? StartDateTime { get; set; }
        [Display(Name = "Ending time")]
        public DateTimeOffset? EndDateTime { get; set; }

        public bool IsUserSignedUpForTask { get; private set; }

        public List<TaskSignupViewModel> AssignedVolunteers { get; set; } = new List<TaskSignupViewModel>();

        public int AcceptedVolunteerCount => AssignedVolunteers?.Where(v => v.Status == "Accepted").Count() ?? 0;

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

        public static AllReadyTask ToModel(this TaskViewModel taskViewModel, IAllReadyDataAccess dataAccess)
        {
            var activity = dataAccess.GetActivity(taskViewModel.ActivityId);

            if (activity == null)
                return null;

            bool newTask = true;
            AllReadyTask dbtask;

            if (taskViewModel.Id == 0)
            {
                dbtask = new AllReadyTask();
            }
            else
            {
                dbtask = dataAccess.GetTask(taskViewModel.Id);
                newTask = false;
            }

            dbtask.Id = taskViewModel.Id;
            dbtask.Description = taskViewModel.Description;
            dbtask.Activity = activity;
            dbtask.EndDateTimeUtc = taskViewModel.EndDateTime.HasValue ? taskViewModel.EndDateTime.Value.UtcDateTime : new Nullable<DateTime>();
            dbtask.StartDateTimeUtc = taskViewModel.EndDateTime.HasValue ? taskViewModel.StartDateTime.Value.UtcDateTime : new Nullable<DateTime>();
            dbtask.Name = taskViewModel.Name;
            dbtask.RequiredSkills = dbtask.RequiredSkills ?? new List<TaskSkill>();
            taskViewModel.RequiredSkills = taskViewModel.RequiredSkills ?? new List<int>();
            //Remove old skills
            dbtask.RequiredSkills.RemoveAll(ts => !taskViewModel.RequiredSkills.Any(s => ts.SkillId == s));
            //Add new skills
            dbtask.RequiredSkills.AddRange(taskViewModel.RequiredSkills
                .Where(rs => !dbtask.RequiredSkills.Any(ts => ts.SkillId == rs))
                .Select(rs => new TaskSkill() { SkillId = rs, TaskId = taskViewModel.Id }));

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
                    foreach (TaskSignup taskUsers in taskUsersList)
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

        public static IEnumerable<AllReadyTask> ToModel(this IEnumerable<TaskViewModel> tasks, IAllReadyDataAccess dataContext)
        {
            return tasks.Select(task => task.ToModel(dataContext));
        }

    }
}
