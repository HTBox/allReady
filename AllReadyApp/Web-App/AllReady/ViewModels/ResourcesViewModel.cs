using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.ViewModels
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
