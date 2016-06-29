using System;
using System.Collections.Generic;
using AllReady.Areas.Admin.Models;
using AllReady.Models;

namespace AllReady.Features.Notifications
{
    public class EventDetailForNotificationModel
    {
        public int EventId { get; set; }
        public string CampaignName { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public List<TaskSummaryModel> Tasks { get; set; }
        public List<CampaignContact> CampaignContacts { get; set; }
        public List<EventSignup> UsersSignedUp { get; set; }
        public EventType EventType { get; set; }
        public Models.Event Event { get; set; }
        public ApplicationUser Volunteer { get; set; }
    }
}