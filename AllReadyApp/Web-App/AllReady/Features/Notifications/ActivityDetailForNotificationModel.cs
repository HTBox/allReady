using System.Collections.Generic;
using AllReady.Areas.Admin.Models;
using AllReady.Models;

namespace AllReady.Features.Notifications
{
    public class ActivityDetailForNotificationModel
    {
        public int ActivityId { get; set; }
        public string CampaignName { get; set; }
        public string ActivityName { get; set; }
        public string Description { get; set; }
        public int NumberOfVolunteersRequired { get; set; }
        public List<TaskSummaryModel> Tasks { get; set; }
        public List<CampaignContact> CampaignContacts { get; set; }
        public List<ActivitySignup> UsersSignedUp { get; set; }
        public ActivityTypes ActivityType { get; set; }
        public Models.Activity Activity { get; set; }
        public ApplicationUser Volunteer { get; set; }
    }
}