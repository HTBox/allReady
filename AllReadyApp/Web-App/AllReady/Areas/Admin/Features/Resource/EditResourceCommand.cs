using AllReady.Areas.Admin.ViewModels.Resource;
using MediatR;

namespace AllReady.Areas.Admin.Features.Resource
{
    public class EditResourceCommand : IAsyncRequest<int>
    {
        public ResourceEditViewModel Resource { get; set; }
    }
}
