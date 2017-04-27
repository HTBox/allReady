using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.ViewModels.Campaign
{
    public class CampaignUsersDetailsViewModel
    {
        [Display(Name = "Campaign")]
        public int CampaignId { get; set; }

        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        public List<CampaignUserViewModel> CampaignManagerList { get; set; }

        public List<CampaignUserViewModel> EventManagerList { get; set; }

        public List<CampaignUserViewModel> VolunteerList { get; set; }
    }
}
