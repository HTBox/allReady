using System.Collections.Generic;
using System.Linq;
using AllReady.ViewModels.Campaign;
using System.Text;

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

            if (organization.Location != null)
            {
                var address = new StringBuilder();

                if (!string.IsNullOrEmpty(organization.Location.Address1)) address.Append(organization.Location.Address1 + ", ");
                if (!string.IsNullOrEmpty(organization.Location.Address2)) address.Append(organization.Location.Address2 + ", ");
                if (!string.IsNullOrEmpty(organization.Location.City)) address.Append(organization.Location.City + ", ");
                if (!string.IsNullOrEmpty(organization.Location.State)) address.Append(organization.Location.State + ", ");
                if (!string.IsNullOrEmpty(organization.Location.PostalCode)) address.Append(organization.Location.PostalCode + ", ");
                if (!string.IsNullOrEmpty(organization.Location.Country)) address.Append(organization.Location.Country + ", ");

                FullAddress = address.ToString().TrimEnd(' ').TrimEnd(',');
            }

            HasPrivacyPolicy = !string.IsNullOrEmpty(organization.PrivacyPolicy);
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string WebUrl { get; set; }

        public string FullAddress { get; set; }

        public string Description { get; set; }
        public string Summary { get; set; }

        public List<CampaignViewModel> Campaigns { get; set; }

        public bool HasPrivacyPolicy { get; set; }
    }
}