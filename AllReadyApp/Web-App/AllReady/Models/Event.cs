﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AllReady.ViewModels.Event;

namespace AllReady.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Display(Name = "Campaign")]
        public int CampaignId { get; set; }

        public Campaign Campaign { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Event Type")]
        public EventType EventType { get; set; }
        [Display(Name = "Start date")]
        public DateTimeOffset StartDateTime { get; set; }

        [Display(Name = "End date")]
        public DateTimeOffset EndDateTime { get; set; }

        public Location Location { get; set; }

        public List<AllReadyTask> Tasks { get; set; } = new List<AllReadyTask>();

        public ApplicationUser Organizer { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Display(Name = "Required skills")]
        public List<EventSkill> RequiredSkills { get; set; } = new List<EventSkill>();

        public bool IsLimitVolunteers { get; set; } = true;

        public bool IsAllowWaitList { get; set; } = false;

        /// <summary>
        /// A short piece of optional text which organizers can use to help generate views/volunteers
        /// </summary>
        [MaxLength(150)]
        public string Headline { get; set; }
        
        public bool IsUserInAnyTask(string userId)
        {
            return Tasks.Any(task => task.AssignedVolunteers.Any(av => av.User.Id == userId));
        }

        public ICollection<Request> Requests { get; set; }
        public ICollection<Itinerary> Itineraries { get; set; }
    }

    public static class EventExtensions
    {
        public static IEnumerable<EventViewModel> ToViewModel(this IEnumerable<Event> campaignEvents)
        {
            return campaignEvents.Select(campaignEvent => new EventViewModel(campaignEvent));
        }
    }
}