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

        //property used to carry if a user is an org admin or not from a GET to a POST action method so we don't have to requery the db
        public bool UserIsOrgAdmin { get; set; }

        //used for setting the title on the event delete page
        public string Title { get; set; }
    }
}
