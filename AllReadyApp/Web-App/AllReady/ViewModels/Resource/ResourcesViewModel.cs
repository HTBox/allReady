using System.Collections.Generic;

namespace AllReady.ViewModels.Resource
{
    public class ResourcesViewModel
    {
        public ResourcesViewModel()
        {
            this.Resources = new List<ResourceViewModel>();
        }
        public List<ResourceViewModel> Resources { get; set; }
    }
}
