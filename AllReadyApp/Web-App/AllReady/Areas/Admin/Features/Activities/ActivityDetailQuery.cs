using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class ActivityDetailQuery : IRequest<ActivityDetailModel>
    {
        public int ActivityId { get; set; }
    }
}
