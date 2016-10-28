using System;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class EditRequestCommandHandler : IAsyncRequestHandler<EditRequestCommand, Guid>
    {
        private readonly AllReadyContext _context;

        public EditRequestCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(EditRequestCommand message)
        {
            var request = await _context.Requests
                .Include(l => l.Event)
                .SingleOrDefaultAsync(t => t.RequestId == message.RequestModel.Id).ConfigureAwait(false) ?? _context.Add(new Request { Source = RequestSource.UI }).Entity;

            request.EventId = message.RequestModel.EventId;
            request.Address = message.RequestModel.Address;
            request.City = message.RequestModel.City;
            request.Name = message.RequestModel.Name;
            request.State = message.RequestModel.State;
            request.Zip = message.RequestModel.Zip;
            request.Email = message.RequestModel.Email;
            request.Phone = message.RequestModel.Phone;

            // todo - longitude and latitude lookup

            await _context.SaveChangesAsync();

            return request.RequestId;
        }
    }
}
