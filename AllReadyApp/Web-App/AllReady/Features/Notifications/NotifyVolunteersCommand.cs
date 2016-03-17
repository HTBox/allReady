using MediatR;

namespace AllReady.Features.Notifications
{
    public class NotifyVolunteersCommand : IAsyncRequest
    {
        public NotifyVolunteersViewModel ViewModel { get; set; }
    }
}
