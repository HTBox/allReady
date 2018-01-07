using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class CreateEventManagerCommandHandler : AsyncRequestHandler<CreateEventManagerCommand>
    {
        private readonly AllReadyContext _context;

        public CreateEventManagerCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(CreateEventManagerCommand message)
        {
            var userExist = _context.EventManagers.Any(e => e.UserId == message.UserId && e.EventId == message.EventId);

            if (userExist) return;

            _context.EventManagers.Add(new EventManager
            {
                EventId = message.EventId,
                UserId = message.UserId
            });

            await _context.SaveChangesAsync();
        }
    }
}
