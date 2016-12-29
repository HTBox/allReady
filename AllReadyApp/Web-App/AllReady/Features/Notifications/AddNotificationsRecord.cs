using AllReady.Models;
using MediatR;
using System.Threading.Tasks;

namespace AllReady.Features.Notifications
{
    public class AddNotificationsRecord : IAsyncNotificationHandler<NotificationSentMessage>
    {
        private readonly AllReadyContext _context;

        public AddNotificationsRecord(AllReadyContext context)
        {
            _context = context;
        }

        public async Task Handle(NotificationSentMessage message)
        {
            _context.Notifications.Add(Notification.FromAddNotificationLogMessage(message));

            await _context.SaveChangesAsync();
        }
    }
}
