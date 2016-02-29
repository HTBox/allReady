using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AllReady.Models;

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

            Title = activity.Name;
            Description = activity.Description;
            ActivityType = activity.ActivityType;

            TimeZoneId = activity.Campaign.TimeZoneId;
            StartDateTime = activity.StartDateTime;
            EndDateTime = activity.EndDateTime;

            if (activity.Location != null)
            {
                Location = new LocationViewModel(activity.Location);
            }

            IsClosed = EndDateTime.UtcDateTime < DateTimeOffset.UtcNow;

            ImageUrl = activity.ImageUrl;

            //TODO Location
            Tasks = activity.Tasks != null
                 ? new List<TaskViewModel>(activity.Tasks.Select(data => new TaskViewModel(data)).OrderBy(task => task.StartDateTime))
                 : new List<TaskViewModel>();

            RequiredSkills = activity.RequiredSkills?.Select(acsk => acsk.Skill).ToList();
        }

        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string Title { get; set; }
        public ActivityTypes ActivityType { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string TimeZoneId { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public LocationViewModel Location { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
        public bool IsUserVolunteeredForActivity { get; set; }
        public List<ApplicationUser> Volunteers { get; set; }
        public string UserId { get; set; }
        public List<Skill> RequiredSkills { get; set; }
        public List<Skill> UserSkills { get; set; }
        public int NumberOfVolunteersRequired { get; set; }
        public ActivitySignupViewModel SignupModel { get; set; }
        public bool IsClosed { get; set; }
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

        public static ActivityViewModel WithUserInfo(this ActivityViewModel viewModel, Activity activity, ClaimsPrincipal user, IAllReadyDataAccess dataAccess)
        {
            if (user.IsSignedIn())
            {
                var userId = user.GetUserId();
                var appUser = dataAccess.GetUser(userId);
                viewModel.UserId = userId;
                viewModel.UserSkills = appUser?.AssociatedSkills?.Select(us => us.Skill).ToList();
                viewModel.IsUserVolunteeredForActivity = dataAccess.GetActivitySignups(viewModel.Id, userId).Any();
                var assignedTasks = activity.Tasks.Where(t => t.AssignedVolunteers.Any(au => au.User.Id == userId)).ToList();
                viewModel.Tasks = new List<TaskViewModel>(assignedTasks.Select(data => new TaskViewModel(data, userId)).OrderBy(task => task.StartDateTime));
                viewModel.SignupModel = new ActivitySignupViewModel()
                {
                    ActivityId = viewModel.Id,
                    UserId = userId,
                    Name = appUser.Name,
                    PreferredEmail = appUser.Email,
                    PreferredPhoneNumber = appUser.PhoneNumber
                };
            }
            else
            {
                viewModel.Tasks = new List<TaskViewModel>();
            }
            return viewModel;
        }
    }
}
