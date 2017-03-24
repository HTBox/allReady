using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Resource
{
    public class ResourceDetailViewModel
    {

        public ResourceDetailViewModel()
        {
             
        }
        public ResourceDetailViewModel(AllReady.Models.Resource resource)
        {
            if (resource == null) return;

            Id = resource.Id;
            CampaignId = resource.CampaignId;
            CampaignName = resource.Campaign?.Name;
            Name = resource.Name;
            Description = resource.Description;
            ResourceUrl = resource.ResourceUrl;
        }

        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }

        [Display(Name = "Resource name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Resource description")]

        public string Description { get; set; }

        [Required]
        [Url]
        [Display(Name = "Resource URL")]
        public string ResourceUrl { get; set; }
    }
}