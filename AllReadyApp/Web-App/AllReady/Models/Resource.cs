using System;

namespace AllReady.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime PublishDateBegin { get; set; }
        public DateTime PublishDateEnd { get; set; }
        public string MediaUrl { get; set; }
        public string ResourceUrl { get; set; }
        public string CategoryTag { get; set; }

        public Campaign Campaign { get; set; }
        public int CampaignId { get; set; }
    }
}
