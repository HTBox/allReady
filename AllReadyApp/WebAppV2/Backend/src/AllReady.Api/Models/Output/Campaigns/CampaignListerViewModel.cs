
using NodaTime;

namespace AllReady.Api.Models.Output.Campaigns
{
    public class CampaignListerViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public bool Hidden { get; set; }

        public LocalDate StartDateTime { get; set; }

        public LocalDate EndDateTime { get; set; }

        public DateTimeZone TimeZone { get; set; }

        public string DatUri { get; set; }
    }
}
