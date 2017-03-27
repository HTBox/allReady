using AllReady.Areas.Admin.ViewModels.Resource;
using MediatR;

namespace AllReady.Areas.Admin.Features.Resource
{
    public class ResourceDetailQuery: IAsyncRequest<ResourceDetailViewModel>
    {
        public int ResourceId { get; set; } 
    }
}
