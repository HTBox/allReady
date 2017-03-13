using System;

namespace AllReady.Models
{
    /// <summary>
    /// Defines Campaign Managers via a many-to-many relationship between Users and Campaigns
    /// </summary>
    public class CampaignManager
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
    }
}
