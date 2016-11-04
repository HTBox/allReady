using AllReady.Areas.Admin.ViewModels.Task;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DetailsQuery : IAsyncRequest<DetailsViewModel>
    {
        public int TaskId { get; set; }
    }
}
