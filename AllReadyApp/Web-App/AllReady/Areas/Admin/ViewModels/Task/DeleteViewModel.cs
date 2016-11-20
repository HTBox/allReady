using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Task
{
    public class DeleteViewModel
    {
        public int Id { get; set; }

        public int OrganizationId { get; set; }

        public int CampaignId { get; set; }

        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }

        public int EventId { get; set; }

        [Display(Name = "Event")]
        public string EventName { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Start date")]
        public DateTimeOffset StartDateTime { get; set; }

        [Display(Name = "End date")]
        public DateTimeOffset EndDateTime { get; set; }

        public bool UserIsOrgAdmin { get; set; }

        public string Title { get; set; }

        /// <summary>List of file attachments</summary>
        public List<FileAttachmentModel> Attachments { get; set; } = new List<FileAttachmentModel>();
    }
}