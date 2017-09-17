using System;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DeleteViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CampaignId { get; set; }

        public string CampaignName { get; set; }

        public int OrganizationId { get; set; }

        [Display(Name = "Start Date")]
        public DateTimeOffset StartDateTime { get; set; }

        [Display(Name = "End Date")]
        public DateTimeOffset EndDateTime { get; set; }
    }
}
