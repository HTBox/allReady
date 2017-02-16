using System;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Resource
{
    public class ResourceCreateViewModel
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }

        [Display(Name = "Resource name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Resource description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Resource URL")]
        [Required]
        public string ResourceUrl { get; set; }
    }
}