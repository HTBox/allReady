using System;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;

namespace AllReady.ViewModels
{
    public class ResourceViewModel
    {
        public ResourceViewModel()
        {
        }

        public ResourceViewModel(Resource resource)
        {
            Id = resource.Id;
            Name = resource.Name;
            Description = resource.Description;
            PublishDateBegin = resource.PublishDateBegin;
            PublishDateEnd = resource.PublishDateEnd;           
            MediaUrl = resource.MediaUrl;
            ResourceUrl = resource.ResourceUrl;
            CategoryTag = resource.CategoryTag;
        }

        public int Id { get; set; }

        [Display(Name = "Resource name")]
        [Required]        
        public string Name { get; set; }

        [Display(Name = "Resource description")]
        public string Description { get; set; }

        [Display(Name = "Resource publish begin date")]
        public DateTime PublishDateBegin { get; set; }

        [Display(Name = "Resource publish end date")]
        public DateTime PublishDateEnd { get; set; }

        [Display(Name = "Resource media URL")]
        public string MediaUrl { get; set; }

        [Display(Name = "Resource URL")]
        public string ResourceUrl { get; set; }

        [Display(Name = "Resource category tag")]
        public string CategoryTag { get; set; }
    }
}
