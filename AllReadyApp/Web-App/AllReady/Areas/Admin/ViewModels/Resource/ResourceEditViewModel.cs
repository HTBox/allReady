using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Resource
{
    public class ResourceEditViewModel
    {
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