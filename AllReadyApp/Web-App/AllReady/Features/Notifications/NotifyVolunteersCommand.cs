using MediatR;

namespace AllReady.Features.Notifications
{
    public class NotifyVolunteersCommand : IRequest
    {
        public NotifyVolunteersViewModel ViewModel { get; set; }
    }
}
