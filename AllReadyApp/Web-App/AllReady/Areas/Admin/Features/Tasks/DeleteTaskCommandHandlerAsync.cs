using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DeleteTaskCommandHandlerAsync : AsyncRequestHandler<DeleteTaskCommandAsync>
    {
        private AllReadyContext _context;

        public DeleteTaskCommandHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(DeleteTaskCommandAsync message)
        {
            var task = _context.Tasks.SingleOrDefault(c => c.Id == message.TaskId);

            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}