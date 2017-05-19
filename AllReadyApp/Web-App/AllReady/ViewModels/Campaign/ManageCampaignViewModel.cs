using System;
using System.Collections.Generic;
using System.Linq;

namespace AllReady.ViewModels.Campaign
{
    public class ManageCampaignViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ManagingOrganizationName { get; set; }

        public string ManagingOrganizationLogo { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public bool IsCampaignManager { get; set; }

        public bool Featured { get; set; }
    }
}