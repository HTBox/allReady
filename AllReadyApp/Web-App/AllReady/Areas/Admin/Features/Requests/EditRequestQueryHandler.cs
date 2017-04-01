using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class EditRequestQueryHandler : IAsyncRequestHandler<EditRequestQuery, EditRequestViewModel>
    {
        private readonly AllReadyContext _context;

        public EditRequestQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EditRequestViewModel> Handle(EditRequestQuery message)
        {
            var request = await _context.Requests
                .Include(l => l.Event).ThenInclude(rec => rec.Campaign).ThenInclude(rec => rec.ManagingOrganization)
                .SingleOrDefaultAsync(t => t.RequestId == message.Id);

            if (request == null)
                return null;

            return new EditRequestViewModel
            {
                Address = request.Address,
                CampaignId = request.Event.CampaignId,
                CampaignName = request.Event.Campaign.Name,
                City = request.City,
                DateAdded = request.DateAdded,
                Email = request.Email,
                EventId = request.Event.Id,
                EventName = request.Event.Name,
                Id = request.RequestId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                OrganizationId = request.Event.Campaign.ManagingOrganizationId,
                Name = request.Name,
                OrganizationName = request.Event.Campaign.ManagingOrganization.Name,
                Phone = request.Phone,
                Status = request.Status,
                State = request.State,
                PostalCode = request.PostalCode,
                Notes = request.Notes
            };
        }
    }
}
