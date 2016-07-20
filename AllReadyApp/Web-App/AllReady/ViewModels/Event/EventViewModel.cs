using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels.Shared;
using AllReady.ViewModels.Task;
using Microsoft.AspNetCore.Identity;

namespace AllReady.ViewModels.Event
{
    public class EventViewModel
    {
        public EventViewModel()
        {
        }

        public EventViewModel(Models.Event @event)
        {
            Id = @event.Id;
            if (@event.Campaign != null)
            {
                CampaignId = @event.Campaign.Id;
                CampaignName = @event.Campaign.Name;
                TimeZoneId = @event.Campaign.TimeZoneId;
                if (@event.Campaign.ManagingOrganization != null)
                {
                    OrganizationId = @event.Campaign.ManagingOrganization.Id;
                    OrganizationName = @event.Campaign.ManagingOrganization.Name;
                    HasPrivacyPolicy = !string.IsNullOrEmpty(@event.Campaign.ManagingOrganization.PrivacyPolicy);
                }
            }

            Title = @event.Name;
            Description = @event.Description;
            EventType = @event.EventType;
            StartDateTime = @event.StartDateTime;
            EndDateTime = @event.EndDateTime;

            if (@event.Location != null)
            {
                Location = new LocationViewModel(@event.Location);
            }

            IsClosed = EndDateTime.UtcDateTime < DateTimeOffset.UtcNow;

            ImageUrl = @event.ImageUrl;

            //TODO Location
            Tasks = @event.Tasks != null
                 ? new List<TaskViewModel>(@event.Tasks.Select(data => new TaskViewModel(data)).OrderBy(task => task.StartDateTime))
                 : new List<TaskViewModel>();

            SignupModel = new EventSignupViewModel();

            RequiredSkills = @event.RequiredSkills?.Select(acsk => new SkillViewModel(acsk.Skill)).ToList();
            IsLimitVolunteers = @event.IsLimitVolunteers;
            IsAllowWaitList = @event.IsAllowWaitList;
            Headline = @event.Headline;
        }

        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string Title { get; set; }
        public EventType EventType { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string TimeZoneId { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public LocationViewModel Location { get; set; }
        public List<TaskViewModel> Tasks { get; set; } = new List<TaskViewModel>();
        public List<TaskViewModel> UserTasks { get; set; } = new List<TaskViewModel>();
        public bool IsUserVolunteeredForEvent { get; set; }
        public List<ApplicationUser> Volunteers { get; set; }
        public string UserId { get; set; }
        public List<SkillViewModel> RequiredSkills { get; set; }
        public List<SkillViewModel> UserSkills { get; set; }
        public int NumberOfVolunteersRequired { get; set; }
        public EventSignupViewModel SignupModel { get; set; }
        public bool IsClosed { get; set; }
        public bool HasPrivacyPolicy { get; set; }
        public List<EventSignup> UsersSignedUp { get; set; } = new List<EventSignup>();
        public bool IsLimitVolunteers { get; set; } = true;
        public bool IsAllowWaitList { get; set; } = true;
        public int NumberOfUsersSignedUp => UsersSignedUp.Count;
        public bool IsFull => NumberOfUsersSignedUp >= NumberOfVolunteersRequired;
        public bool IsAllowSignups => !IsLimitVolunteers || !IsFull || IsAllowWaitList;

        public string Headline { get; set; }
        public bool HasHeadline => !string.IsNullOrEmpty(Headline);
    }
}
