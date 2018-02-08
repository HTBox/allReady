using System.Linq;
using System.Threading.Tasks;

using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Models;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditVolunteerTaskQueryHandler : IAsyncRequestHandler<EditVolunteerTaskQuery, EditViewModel>
    {
        private AllReadyContext _context;

        public EditVolunteerTaskQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EditViewModel> Handle(EditVolunteerTaskQuery message)
        {
            return await _context.VolunteerTasks.AsNoTracking()
                .Include(t => t.Event).ThenInclude(a => a.Campaign)
                .Include(t => t.RequiredSkills).ThenInclude(ts => ts.Skill)
                .Include(t => t.Attachments)
                .Select(task => new EditViewModel
                {
                    Id = task.Id,
                    Name = task.Name,
                    Description = task.Description,
                    StartDateTime = task.StartDateTime,
                    EndDateTime = task.EndDateTime,
                    NumberOfVolunteersRequired = task.NumberOfVolunteersRequired,
                    RequiredSkills = task.RequiredSkills,
                    EventId = task.Event.Id,
                    EventName = task.Event.Name,
                    EventStartDate = task.Event.StartDateTime,
                    EventEndDate = task.Event.EndDateTime,
                    CampaignId = task.Event.CampaignId,
                    CampaignName = task.Event.Campaign.Name,
                    OrganizationId = task.Event.Campaign.ManagingOrganizationId,
                    TimeZoneId = task.Event.TimeZoneId,
                    Attachments = task.Attachments.Select(a => new FileAttachment
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description,
                        Url = a.Url,
                    }).ToList(),

                })
                .SingleAsync(t => t.Id == message.VolunteerTaskId);
        }
    }
}
