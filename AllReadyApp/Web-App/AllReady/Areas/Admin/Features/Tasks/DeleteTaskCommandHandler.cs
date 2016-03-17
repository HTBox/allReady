using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DeleteTaskCommandHandler : RequestHandler<DeleteTaskCommand>
    {
        private AllReadyContext _context;

        public DeleteTaskCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override void HandleCore(DeleteTaskCommand message)
        {
            var task = _context.Tasks.SingleOrDefault(c => c.Id == message.TaskId);

            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
        }
    }
}
