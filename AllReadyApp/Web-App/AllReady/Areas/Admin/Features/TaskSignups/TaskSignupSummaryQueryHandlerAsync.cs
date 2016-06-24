using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models.TaskSignupModels;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.TaskSignups
{
    public class TaskSignupSummaryQueryHandlerAsync : IAsyncRequestHandler<TaskSignupSummaryQuery, TaskSignupSummaryModel>
    {
        private readonly AllReadyContext _context;

        public TaskSignupSummaryQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<TaskSignupSummaryModel> Handle(TaskSignupSummaryQuery message)
        {
            return await _context.TaskSignups.AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.Id == message.TaskSignupId)
                .Select(
                    x =>
                        new TaskSignupSummaryModel
                        {
                            TaskSignupId = x.Id,
                            VolunteerName = x.User.Name,
                            VolunteerEmail = x.PreferredEmail ?? x.User.Email
                        })
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }
    }
}
