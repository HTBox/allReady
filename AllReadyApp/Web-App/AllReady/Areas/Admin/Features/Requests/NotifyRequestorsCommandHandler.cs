using System;
using System.Threading.Tasks;
using AllReady.Services;
using MediatR;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class NotifyRequestorsCommandHandler : IAsyncRequestHandler<NotifyRequestorsCommand, bool>
    {
        private readonly IQueueStorageService _queue;
        private readonly IMediator _mediator;

        public NotifyRequestorsCommandHandler( IQueueStorageService queue, IMediator mediator )
        {
            _queue = queue;
            _mediator = mediator;
        }

        public async Task<bool> Handle( NotifyRequestorsCommand message )
        {
            foreach (var request in message.Requests)
            {
                var notificationMessage = message.NotificationMessageBuilder(request, message.Itinerary);

                //Todo: We need to add communication preferences to the request object - we naively assume texting the requestor is both desired and a good thing
                await _queue.SendMessageAsync(QueueStorageService.Queues.SmsQueue, notificationMessage);
            }

            return true;
        }
    }
}
