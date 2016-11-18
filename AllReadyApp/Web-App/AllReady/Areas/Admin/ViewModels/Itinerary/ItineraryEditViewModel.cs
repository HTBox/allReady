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

        [Display(Name = "Address line 1")]
        public string StartAddress1 { get; set; }

        [Display(Name = "Address line 2")]
        public string StartAddress2 { get; set; }

        [Display(Name = "City")]
        public string StartCity { get; set; }

        [Display(Name = "State")]
        public string StartState { get; set; }

        [Display(Name = "Postal Code")]
        public string StartPostalCode { get; set; }

        [Display(Name = "Country")]
        public string StartCountry { get; set; }

        [Display(Name = "Address line 1")]
        public string EndAddress1 { get; set; }

        [Display(Name = "Address line 2")]
        public string EndAddress2 { get; set; }

        [Display(Name = "City")]
        public string EndCity { get; set; }

        [Display(Name = "State")]
        public string EndState { get; set; }

        [Display(Name = "Postal Code")]
        public string EndPostalCode { get; set; }

        [Display(Name = "Country")]
        public string EndCountry { get; set; }

        [Display(Name = "Same as start address")]
        public bool UseStartAddressAsEndAddress { get; set; } = true;
    }
}
