using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class EditActivityCommand : IAsyncRequest<int>
    {
        public ActivityDetailModel Activity {get; set;}
    }
}
