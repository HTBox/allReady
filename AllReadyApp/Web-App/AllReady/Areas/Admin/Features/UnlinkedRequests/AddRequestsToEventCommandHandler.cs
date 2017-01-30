using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;

namespace AllReady.Areas.Admin.Features.UnlinkedRequests
{
    public class AddRequestsToEventCommandHandler : IAsyncRequestHandler<AddRequestsToEventCommand, bool>
    {
        private readonly AllReadyContext _context;

        public AddRequestsToEventCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AddRequestsToEventCommand message)
        {
            var selectedEvent = await _context.Events
                .Where(x => x.Id == message.EventId)
                .Select(x => new {x.Id, x.Name})
                .SingleOrDefaultAsync();

            // todo: enhance this with a error message so the controller can better respond to the issue
            if (selectedEvent == null) return false;

            var requestsToUpdate = await _context.Requests.AsAsyncEnumerable()
                .Where(r => message.SelectedRequestIds.Contains(r.RequestId))
                .ToList();

            // todo: we should enhance the returned object to include a message so that the controller can provide better feedback to the user
            if (!requestsToUpdate.Any()) return false;

            requestsToUpdate.ForEach(request => request.EventId = selectedEvent.Id);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}       