using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Events
{
    public class UpdateEventImageUrlHandler : AsyncRequestHandler<UpdateEventImageUrl>
    {
        private readonly AllReadyContext _context;

        public UpdateEventImageUrlHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(UpdateEventImageUrl message)
        {
            var @event = await _context.Events.SingleAsync(x => x.Id == message.EventId);
            @event.ImageUrl = message.ImageUrl;

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
