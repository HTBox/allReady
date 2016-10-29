using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DeleteEventCommandHandler : AsyncRequestHandler<DeleteEventCommand>
    {
        private AllReadyContext _context;

        public DeleteEventCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(DeleteEventCommand message)
        {
            var @event = _context.Events.SingleOrDefault(c => c.Id == message.EventId);
            if (@event != null)
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
            }
        }
    }
}