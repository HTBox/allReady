using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Resource
{
    public class ResourceDeleteViewModel
    {
        public ResourceDeleteViewModel()
        {
             
        }
        public ResourceDeleteViewModel(AllReady.Models.Resource resource)
        {
            if (resource == null) return;

            Id = resource.Id;
            CampaignId = resource.CampaignId;
            Name = resource.Name;
            ResourceUrl = resource.ResourceUrl;
        }

        public int Id { get; set; }
        public int CampaignId { get; set; }
        public bool UserIsOrgAdmin { get; set; }
        [Display(Name="Resource Name")]
        public string Name { get; set; }
        [Display(Name="Resource Url")]
        public string ResourceUrl { get; set; }
    }
}