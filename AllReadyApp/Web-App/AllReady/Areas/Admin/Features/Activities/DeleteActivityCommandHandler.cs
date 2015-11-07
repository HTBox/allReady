using AllReady.Models;
using MediatR;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class DeleteActivityCommandHandler : RequestHandler<DeleteActivityCommand>
    {
        private AllReadyContext _context;

        public DeleteActivityCommandHandler(AllReadyContext context)
        {
            _context = context;

        }
        protected override void HandleCore(DeleteActivityCommand message)
        {
            var activity = 
                _context.Activities.SingleOrDefault(c => c.Id == message.ActivityId);

            if (activity != null)
            {
                _context.Activities.Remove(activity);
                _context.SaveChanges();
            }
        }
    }
}
