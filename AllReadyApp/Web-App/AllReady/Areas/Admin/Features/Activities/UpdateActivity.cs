using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class UpdateActivity : IAsyncRequest
    {
        public Activity Activity { get; set; }
    }
}
