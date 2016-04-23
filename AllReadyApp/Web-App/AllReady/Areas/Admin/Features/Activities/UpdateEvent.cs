using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class UpdateEvent : IAsyncRequest
    {
        public Event Event { get; set; }
    }
}
