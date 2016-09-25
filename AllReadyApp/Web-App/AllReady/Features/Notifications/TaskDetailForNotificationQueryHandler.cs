using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Notifications
{
    public class TaskDetailForNotificationQueryHandler : IAsyncRequestHandler<TaskDetailForNotificationQuery, TaskDetailForNotificationModel>
    {
        private readonly AllReadyContext _context;

        public TaskDetailForNotificationQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<TaskDetailForNotificationModel> Handle(TaskDetailForNotificationQuery message)
        {
            TaskDetailForNotificationModel result = null;

            var task = await GetTask(message).ConfigureAwait(false);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == message.UserId).ConfigureAwait(false);
            
            if (task != null && user != null)
            {
                result = new TaskDetailForNotificationModel
                {
                    TaskId = task.Id,
                    TaskName = task.Name,
                    EventId = task.EventId,
                    EventType = task.Event.EventType,
                    EventName = task.Event.Name,
                    CampaignName = task.Event.Campaign.Name,
                    CampaignContacts = task.Event.Campaign.CampaignContacts,
                    Volunteer = user,
                    Description = task.Description,
                    NumberOfVolunteersRequired = task.NumberOfVolunteersRequired,
                    TaskStartDate = task.StartDateTime,
                    NumberOfVolunteersSignedUp = task.NumberOfUsersSignedUp
                };
            }

            return result;
        }

        private async Task<AllReadyTask> GetTask(TaskDetailForNotificationQuery message)
        {
            return await _context.Tasks.AsNoTracking()
                .Include(a => a.Event).ThenInclude(e => e.Campaign).ThenInclude(c => c.CampaignContacts).ThenInclude(cc => cc.Contact)
                .Include(a => a.AssignedVolunteers).ThenInclude(a => a.User)
                .SingleOrDefaultAsync(a => a.Id == message.TaskId).ConfigureAwait(false);
        }
    }
}
