using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DeleteVolunteerTaskCommandHandler : AsyncRequestHandler<DeleteVolunteerTaskCommand>
    {
        private AllReadyContext _context;

        public DeleteVolunteerTaskCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(DeleteVolunteerTaskCommand message)
        {
            var volunteerTask = _context.VolunteerTasks.SingleOrDefault(c => c.Id == message.VolunteerTaskId);

            if (volunteerTask != null)
            {
                _context.VolunteerTasks.Remove(volunteerTask);
                await _context.SaveChangesAsync();
            }
        }
    }
}