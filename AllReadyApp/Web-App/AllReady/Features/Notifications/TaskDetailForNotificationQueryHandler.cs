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

            var theTask = await GetTask(message);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == message.UserId);
            
            if (theTask != null && user != null)
            {
                result = new TaskDetailForNotificationModel
                {
                    TaskId = theTask.Id,
                    TaskName = theTask.Name,
                    EventId = theTask.EventId,
                    EventType = theTask.Event.EventType,
                    EventName = theTask.Event.Name,
                    CampaignName = theTask.Event.Campaign.Name,
                    CampaignContacts = theTask.Event.Campaign.CampaignContacts,
                    Volunteer = user,
                    Description = theTask.Description,
                    NumberOfVolunteersRequired = theTask.NumberOfVolunteersRequired,
                    TaskStartDate = theTask.StartDateTime,
                    NumberOfVolunteersSignedUp = theTask.NumberOfUsersSignedUp
                };
            }

            return result;
        }

        private async Task<AllReadyTask> GetTask(TaskDetailForNotificationQuery message)
        {
            return await _context.Tasks.AsNoTracking()
                .Include(a => a.Event).ThenInclude(e => e.Campaign).ThenInclude(c => c.CampaignContacts).ThenInclude(cc => cc.Contact)
                .Include(a => a.AssignedVolunteers).ThenInclude(a => a.User)
                .SingleOrDefaultAsync(a => a.Id == message.TaskId);
        }
    }
}
