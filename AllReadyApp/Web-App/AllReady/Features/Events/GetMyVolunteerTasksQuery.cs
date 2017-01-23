using System.Collections.Generic;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Features.Events
{
    public class GetMyVolunteerTasksQuery : IRequest<IEnumerable<VolunteerTaskSignupViewModel>>
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
    }
}
