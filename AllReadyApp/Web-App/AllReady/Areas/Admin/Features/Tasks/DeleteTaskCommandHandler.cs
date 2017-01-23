using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DeleteTaskCommandHandler : AsyncRequestHandler<DeleteTaskCommand>
    {
        private AllReadyContext _context;

        public DeleteTaskCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(DeleteTaskCommand message)
        {
            var @task = _context.Tasks
                .Include(t => t.Attachments)
                .SingleOrDefault(c => c.Id == message.TaskId);

            if (@task != null)
            {
                _context.Attachments.RemoveRange(task.Attachments);
                _context.Tasks.Remove(@task);
                await _context.SaveChangesAsync();
            }
        }
    }
}