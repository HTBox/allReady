using System.Linq;
using System.Threading.Tasks;

using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Models;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DeleteQueryHandler : IAsyncRequestHandler<DeleteQuery, DeleteViewModel>
    {
        private readonly AllReadyContext _context;

        public DeleteQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<DeleteViewModel> Handle(DeleteQuery message)
        {
            return await _context.VolunteerTasks
                .AsNoTracking()
                .Include(t => t.Event.Campaign)
                .Include(t => t.Attachments)
                .Select(task => new DeleteViewModel
                {
                    Id = task.Id,
                    OrganizationId = task.Event.Campaign.ManagingOrganizationId,
                    CampaignId = task.Event.CampaignId,
                    CampaignName = task.Event.Campaign.Name,
                    EventId = task.Event.Id,
                    EventName = task.Event.Name,
                    Name = task.Name,
                    StartDateTime = task.StartDateTime,
                    EndDateTime = task.EndDateTime,
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