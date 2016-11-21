using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ChangeRequestStatusCommandHandler : IAsyncRequestHandler<ChangeRequestStatusCommand, bool>
    {
        private readonly AllReadyContext _context;

        public ChangeRequestStatusCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ChangeRequestStatusCommand message)
        {
            var req = new Request
            {
                RequestId = message.RequestId,
                Status = message.NewStatus
            };

            _context.Requests.Attach(req);
            _context.Entry(req).Property(x => x.Status).IsModified = true;
            var changedCount = await _context.SaveChangesAsync();

            return changedCount > 0;
        }
    }
}
