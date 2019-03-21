using AllReady.Api.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Api.Models.Input
{
    public class CreateCampaignInputModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }

        [Required]
        public string ShortDescription { get; set; }

        public string FullDescription { get; set; }

        [Required]
        public string TimeZoneId { get; set; }

        public string ImageUrl { get; set; }
    }
}
