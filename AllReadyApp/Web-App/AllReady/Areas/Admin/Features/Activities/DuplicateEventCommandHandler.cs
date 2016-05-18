using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DuplicateEventCommandHandler : IAsyncRequestHandler<DuplicateEventCommand, int>
    {
        private AllReadyContext _context;

        public DuplicateEventCommandHandler(AllReadyContext context)
        {
            _context = context;

        }
        public async Task<int> Handle(DuplicateEventCommand message)
        {
            var fromEvent = await GetEvent(message.FromEventId);
            var toEvent = await GetEvent(message.ToEventId);

            if (fromEvent.Tasks.Any())
            {
                foreach (var task in fromEvent.Tasks)
                {
                    task.Id = 0;
                    task.Event = toEvent;

                    // Todo: Check if this handles timezones correctly.
                    if (task.StartDateTime.HasValue)
                        task.StartDateTime = toEvent.StartDateTime - (fromEvent.StartDateTime - task.StartDateTime.Value);

                    if (task.EndDateTime.HasValue)
                        task.EndDateTime = toEvent.EndDateTime - (fromEvent.EndDateTime - task.EndDateTime.Value);

                    _context.Tasks.Add(task);
                }
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return toEvent.Id;
        }

        private async Task<Event> GetEvent(int eventId)
        {
            return await _context.Events
                .Include(a => a.Tasks)
                .SingleOrDefaultAsync(c => c.Id == eventId)
                .ConfigureAwait(false);
        }
    }
}