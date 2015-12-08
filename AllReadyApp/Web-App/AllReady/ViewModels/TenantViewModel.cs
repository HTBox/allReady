using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.ViewModels
{
    public class TenantViewModel
    {
        public TenantViewModel()
        {
            Campaigns = new List<CampaignViewModel>();
        }

        public TenantViewModel(Organization tenant)
        {
            Id = tenant.Id;
            Name = tenant.Name;
            LogoUrl = tenant.LogoUrl;
            WebUrl = tenant.WebUrl;

            if (tenant.Campaigns != null)
            {
                Campaigns = tenant.Campaigns.ToViewModel().ToList();
            }
            else
            {
                Campaigns = new List<CampaignViewModel>();
            }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string WebUrl { get; set; }

        public List<CampaignViewModel> Campaigns { get; set; }
    }    
}
