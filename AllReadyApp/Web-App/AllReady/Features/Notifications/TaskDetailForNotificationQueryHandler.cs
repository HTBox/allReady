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

            var volunteerTask = await GetTask(message);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == message.UserId);
            
            if (volunteerTask != null && user != null)
            {
                result = new TaskDetailForNotificationModel
                {
                    VolunteerTaskId = volunteerTask.Id,
                    VolunteerTaskName = volunteerTask.Name,
                    EventId = volunteerTask.EventId,
                    EventType = volunteerTask.Event.EventType,
                    EventName = volunteerTask.Event.Name,
                    CampaignName = volunteerTask.Event.Campaign.Name,
                    CampaignContacts = volunteerTask.Event.Campaign.CampaignContacts,
                    Volunteer = user,
                    Description = volunteerTask.Description,
                    NumberOfVolunteersRequired = volunteerTask.NumberOfVolunteersRequired,
                    VolunteerTaskStartDate = volunteerTask.StartDateTime,
                    NumberOfVolunteersSignedUp = volunteerTask.NumberOfUsersSignedUp
                };
            }

            return result;
        }

        private async Task<VolunteerTask> GetTask(TaskDetailForNotificationQuery message)
        {
            return await _context.Tasks.AsNoTracking()
                .Include(a => a.Event).ThenInclude(e => e.Campaign).ThenInclude(c => c.CampaignContacts).ThenInclude(cc => cc.Contact)
                .Include(a => a.AssignedVolunteers).ThenInclude(a => a.User)
                .SingleOrDefaultAsync(a => a.Id == message.VolunteerTaskId);
        }
    }
}
