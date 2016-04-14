using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class DeleteActivityCommandHandler : AsyncRequestHandler<DeleteActivityCommand>
    {
        private AllReadyContext _context;

        public DeleteActivityCommandHandler(AllReadyContext context)
        {
            _context = context;

        }

        protected override async Task HandleCore(DeleteActivityCommand message)
        {
            var activity =  _context.Activities.SingleOrDefault(c => c.Id == message.ActivityId);
            if (activity != null)
            {
                _context.Activities.Remove(activity);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}