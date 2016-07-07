using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class RequestStatusChangeCommandHandlerAsync : IAsyncRequestHandler<RequestStatusChangeCommand, bool>
    {
        private readonly AllReadyContext _context;

        public RequestStatusChangeCommandHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RequestStatusChangeCommand message)
        {
            var req = new Request
            {
                RequestId = message.RequestId,
                Status = message.NewStatus
            };

            _context.Requests.Attach(req);
            _context.Entry(req).Property(x => x.Status).IsModified = true;
            var changedCount = await _context.SaveChangesAsync().ConfigureAwait(false);

            return changedCount > 0;
        }
    }
}
