using System.Collections.Generic;
using System.Linq;
using AllReady.Models;

namespace AllReady.ViewModels
{
    public class OrganizationViewModel
    {
        public OrganizationViewModel()
        {
            Campaigns = new List<CampaignViewModel>();
        }

        public OrganizationViewModel(Organization organization)
        {
            Id = organization.Id;
            Name = organization.Name;
            LogoUrl = organization.LogoUrl;
            WebUrl = organization.WebUrl;

            if (organization.Campaigns != null)
            {
                Campaigns = organization.Campaigns.ToViewModel().ToList();
            }
            else
            {
                Campaigns = new List<CampaignViewModel>();
            }

            if (!string.IsNullOrEmpty(organization.PrivacyPolicy))
            {
                HasPrivacyPolicy = true;
            }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string WebUrl { get; set; }

        public List<CampaignViewModel> Campaigns { get; set; }

        public bool HasPrivacyPolicy { get; set; }
    }    
}