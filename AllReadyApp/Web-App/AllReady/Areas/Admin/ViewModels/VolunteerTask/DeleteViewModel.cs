using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.VolunteerTask
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

        [Display(Name = "Start Date")]
        public DateTimeOffset StartDateTime { get; set; }

        [Display(Name = "End Date")]
        public DateTimeOffset EndDateTime { get; set; }

        public bool UserIsOrgAdmin { get; set; }

        public string Title { get; set; }

        /// <summary>List of file attachments</summary>
        [Display(Name = "Attachments")]
        public List<FileAttachment> Attachments { get; set; } = new List<FileAttachment>();
    }
}