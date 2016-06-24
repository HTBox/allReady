using System.Collections.Generic;
using System.Linq;
using AllReady.ViewModels.Campaign;

namespace AllReady.ViewModels.Organization
{
    public class OrganizationViewModel
    {
        public OrganizationViewModel()
        {
            Campaigns = new List<CampaignViewModel>();
        }

        public OrganizationViewModel(Models.Organization organization)
        {
            Id = organization.Id;
            Name = organization.Name;
            LogoUrl = organization.LogoUrl;
            WebUrl = organization.WebUrl;
            Description = organization.DescriptionHtml;
            Summary = organization.Summary;

            if (organization.Campaigns != null)
            {
                Campaigns = organization.Campaigns.ToViewModel().ToList();
            }
            else
            {
                Campaigns = new List<CampaignViewModel>();
            }

            HasPrivacyPolicy = !string.IsNullOrEmpty(organization.PrivacyPolicy);
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string WebUrl { get; set; }

        public string Description { get; set; }
        public string Summary { get; set; }

        public List<CampaignViewModel> Campaigns { get; set; }

        public bool HasPrivacyPolicy { get; set; }
    }    
}