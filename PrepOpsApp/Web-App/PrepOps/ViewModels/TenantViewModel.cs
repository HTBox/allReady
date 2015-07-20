using PrepOps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrepOps.ViewModels
{
    public class TenantViewModel
    {
        public TenantViewModel()
        {
            Campaigns = new List<CampaignViewModel>();
        }

        public TenantViewModel(Tenant tenant)
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

    public static class TenantViewModelExtensions
    {
        public static TenantViewModel ToViewModel(this Tenant campaign)
        {
            return new TenantViewModel(campaign);
        }

        public static IEnumerable<TenantViewModel> ToViewModel(this IEnumerable<Tenant> tenants)
        {
            return tenants.Select(x => x.ToViewModel());
        }

        public static Tenant ToModel(this TenantViewModel tenant, IPrepOpsDataAccess dataAccess)
        {
            return new Tenant
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Campaigns = tenant.Campaigns.ToModel(dataAccess).ToList(),
                LogoUrl = tenant.LogoUrl,
                WebUrl = tenant.WebUrl
            };
        }

        public static IEnumerable<Tenant> ToModel(this IEnumerable<TenantViewModel> tenants, IPrepOpsDataAccess dataAccess)
        {
            return tenants.Select(x => x.ToModel(dataAccess));
        }
    }
}
