using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.TaskSignup;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.TaskSignups
{
    public class TaskSignupSummaryQueryHandlerAsync : IAsyncRequestHandler<TaskSignupSummaryQuery, TaskSignupSummaryViewModel>
    {
        private readonly AllReadyContext _context;

        public TaskSignupSummaryQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<TaskSignupSummaryViewModel> Handle(TaskSignupSummaryQuery message)
        {
            return await _context.TaskSignups.AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.Id == message.TaskSignupId)
                .Select(
                    x =>
                        new TaskSignupSummaryViewModel
                        {
                            TaskSignupId = x.Id,
                            VolunteerName = x.User.Name,
                            VolunteerEmail = x.PreferredEmail ?? x.User.Email
                        })
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }
    }
}
