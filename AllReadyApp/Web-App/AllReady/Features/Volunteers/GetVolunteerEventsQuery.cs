using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Volunteers
{
    public class GetVolunteerEventsQuery : IAsyncRequest<MyEventsListerViewModel>
    {
        public string UserId { get; set; }
    }
}
