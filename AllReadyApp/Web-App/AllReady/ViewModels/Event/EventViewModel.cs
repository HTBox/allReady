using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels.Shared;
using AllReady.ViewModels.Task;

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
            TimeZoneId = @event.TimeZoneId;
            StartDateTime = @event.StartDateTime;
            EndDateTime = @event.EndDateTime;

            if (@event.Location != null)
            {
                Location = new LocationViewModel(@event.Location);
            }

            IsClosed = EndDateTime.UtcDateTime < DateTimeOffset.UtcNow;

            ImageUrl = @event.ImageUrl;

            Tasks = @event.VolunteerTasks!= null
                 ? new List<VolunteerTaskViewModel>(@event.VolunteerTasks.Select(data => new VolunteerTaskViewModel(data)).OrderBy(task => task.StartDateTime))
                 : new List<VolunteerTaskViewModel>();

            SignupModel = new Shared.VolunteerTaskSignupViewModel();

            //mgmccarthy: this check doesn't make much sense unless you explicitly set @event.RequiredSkills to null. If you look at the Event model, you'll see that RequireSkills is instaniated with
            //a new empty list: "public List<EventSkill> RequiredSkills { get; set; } = new List<EventSkill>();". I think this can go away?
            RequiredSkills = @event.RequiredSkills?.Select(ek => new SkillViewModel(ek.Skill)).ToList();

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
        public List<VolunteerTaskViewModel> Tasks { get; set; } = new List<VolunteerTaskViewModel>();
        public List<VolunteerTaskViewModel> UserTasks { get; set; } = new List<VolunteerTaskViewModel>();
        public string UserId { get; set; }
        public List<SkillViewModel> RequiredSkills { get; set; }
        public List<SkillViewModel> UserSkills { get; set; }
        public Shared.VolunteerTaskSignupViewModel SignupModel { get; set; }
        public bool IsClosed { get; set; }
        public bool HasPrivacyPolicy { get; set; }
        public bool IsLimitVolunteers { get; set; } = true;
        public bool IsAllowWaitList { get; set; } = true;
        public string Headline { get; set; }
    }
}