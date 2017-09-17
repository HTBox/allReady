using System;

namespace AllReady.ViewModels.Home
{
    public class ActiveOrUpcomingEvent
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string CampaignName { get; set; }
        public string CampaignManagedOrganizerName { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}