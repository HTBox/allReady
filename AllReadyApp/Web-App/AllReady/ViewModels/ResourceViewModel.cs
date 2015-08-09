using AllReady.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.ViewModels
{
    public class ResourceViewModel
    {
        public ResourceViewModel()
        {
        }
        public ResourceViewModel(Resource resource)
        {
            this.Id = resource.Id;
            this.Name = resource.Name;
            this.Description = resource.Description;
            this.PublishDateBegin = resource.PublishDateBegin;
            this.PublishDateEnd = resource.PublishDateEnd;           
            this.MediaUrl = resource.MediaUrl;
            this.ResourceUrl = resource.ResourceUrl;
            this.CategoryTag = resource.CategoryTag;
            
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
