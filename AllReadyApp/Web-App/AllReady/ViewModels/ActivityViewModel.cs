using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllReady.ViewModels
{
    public class ActivityViewModel
    {
        public ActivityViewModel()
        {
            this.Tasks = new List<TaskViewModel>();
        }

        public ActivityViewModel(Activity activity)
        {
            Id = activity.Id;
            if (activity.Campaign != null)
            {
                CampaignId = activity.Campaign.Id;
                CampaignName = activity.Campaign.Name;
            }

            if (activity.Tenant != null)
            {
                TenantId = activity.Tenant.Id;
                TenantName = activity.Tenant.Name;
            }

            Title = activity.Name;
            Description = activity.Description;

            StartDateTime = new DateTimeOffset(activity.StartDateTimeUtc, TimeSpan.Zero);
            EndDateTime = new DateTimeOffset(activity.EndDateTimeUtc, TimeSpan.Zero);

            if (activity.Location != null)
            {
                Location = new LocationViewModel(activity.Location);
            }

            ImageUrl = activity.ImageUrl;

            //TODO Location
            Tasks = activity.Tasks != null
                ? new List<TaskViewModel>(activity.Tasks.Select(data => new TaskViewModel(data)).OrderBy(task => task.StartDateTime))
                : new List<TaskViewModel>();

            RequiredSkills = activity.RequiredSkills.Select(acsk => acsk.Skill).ToList();
        }

        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public LocationViewModel Location { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
        public bool IsUserVolunteeredForActivity { get; set; }
        public List<ApplicationUser> Volunteers { get; set; }
        public string UserId { get; set; }
        public List<Skill> RequiredSkills { get; set; }
        public List<Skill> UserSkills { get; set; }
    }

    public static class ActivityViewModelExtension
    {
        public static LocationViewModel ToViewModel(this Location location)
        {
            LocationViewModel value = new LocationViewModel()
            {
                Address1 = location.Address1,
                Address2 = location.Address2,
                City = location.City,
                PostalCode = location.PostalCode,
                State = location.State
            };
            return value;
        }
        public static Location ToModel(this LocationViewModel location)
        {
            Location value = new Location()
            {
                Address1 = location.Address1,
                Address2 = location.Address2,
                City = location.City,
                PostalCode = location.PostalCode,
                State = location.State,
                Country = "TODO:  Put country in both objects"
            };
            return value;
        }
        public static IEnumerable<ActivityViewModel> ToViewModel(this IEnumerable<Activity> activities)
        {
            return activities.Select(activity => new ActivityViewModel(activity));
        }

        /// <summary>
        /// Returns null when there is no matching campaign for the campaign Id.
        /// </summary>
        public static Activity ToModel(this ActivityViewModel activity, IAllReadyDataAccess dataAccess)
        {
            var campaign = dataAccess.GetCampaign(activity.CampaignId);

            if (campaign == null)
                return null;

            Activity newActivity = new Activity()
            {
                Id = activity.Id,
                Campaign = campaign,
                EndDateTimeUtc = activity.EndDateTime.UtcDateTime,
                StartDateTimeUtc = activity.StartDateTime.UtcDateTime,
                Location = new Location()
                {
                    Address1 = activity.Location.Address1,
                    Address2 = activity.Location.Address2,
                    City = activity.Location.City,
                    Country = "US",
                    PostalCode = activity.Location.PostalCode,
                    State = activity.Location.State
                },
                Name = activity.Title
            };
            var tasks = new List<AllReadyTask>();

            foreach (TaskViewModel tvm in activity.Tasks)
            {
                tasks.Add(new AllReadyTask()
                {
                    Activity = newActivity,
                    Name = tvm.Name,
                    Id = tvm.Id,
                    Description = tvm.Description
                });
            }
            newActivity.Tasks = tasks;
            return newActivity;
        }

        public static IEnumerable<Activity> ToModel(this IEnumerable<ActivityViewModel> activities, IAllReadyDataAccess dataAccess)
        {
            return activities.Select(activity => activity.ToModel(dataAccess));
        }

        public static ActivityViewModel WithUserInfo(this ActivityViewModel viewModel, Activity activity, ClaimsPrincipal user, IAllReadyDataAccess dataAccess)
        {
            if (user.IsSignedIn())
            {
                var userId = user.GetUserId();
                viewModel.UserId = userId;
                viewModel.UserSkills = dataAccess.GetUser(userId).AssociatedSkills.Select(us => us.Skill).ToList();
                viewModel.IsUserVolunteeredForActivity = dataAccess.GetActivitySignups(viewModel.Id, userId).Any();
                var assignedTasks = activity.Tasks.Where(t => t.AssignedVolunteers.Any(au => au.User.Id == userId)).ToList();
                viewModel.Tasks = new List<TaskViewModel>(assignedTasks.Select(data => new TaskViewModel(data, userId)).OrderBy(task => task.StartDateTime));
            }
            else
            {
                viewModel.Tasks = new List<TaskViewModel>();
            }
            return viewModel;
        }
    }
}
