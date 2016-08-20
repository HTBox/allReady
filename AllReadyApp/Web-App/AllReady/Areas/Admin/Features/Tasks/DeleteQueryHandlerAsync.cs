using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AllReady.Areas.Admin.ViewModels.Task;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DeleteQueryHandlerAsync : IAsyncRequestHandler<DeleteQueryAsync, DeleteViewModel>
    {
        private readonly AllReadyContext _context;

        public DeleteQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<DeleteViewModel> Handle(DeleteQueryAsync message)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Include(t => t.Event.Campaign)
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
                })
                .SingleAsync(t => t.Id == message.TaskId)
                .ConfigureAwait(false);
        }
    }
}