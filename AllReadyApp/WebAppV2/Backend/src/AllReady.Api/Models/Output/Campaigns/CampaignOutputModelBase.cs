using NodaTime;

namespace AllReady.Api.Models.Output.Campaigns
{
    public abstract class CampaignOutputModelBase
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public LocalDate StartDateTime { get; set; }

        public LocalDate EndDateTime { get; set; }

        public DateTimeZone TimeZone { get; set; }

        public bool IsFeatured { get; set; }

        public string ImageUrl { get; set; }
    }
}
