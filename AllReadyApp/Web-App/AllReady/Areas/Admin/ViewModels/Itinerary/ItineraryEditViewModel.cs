using System;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Itinerary
{
    public class ItineraryEditViewModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public int OrganizationId { get; set; }

        public int CampaignId { get; set; }

        public string CampaignName { get; set; }

        public string EventName { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        
        // todo - address
    }
}
