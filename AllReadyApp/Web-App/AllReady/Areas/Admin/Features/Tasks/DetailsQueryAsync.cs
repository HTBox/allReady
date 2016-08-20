using AllReady.Areas.Admin.ViewModels.Task;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DetailsQueryAsync : IAsyncRequest<DetailsViewModel>
    {
        public int TaskId { get; set; }
    }
}
