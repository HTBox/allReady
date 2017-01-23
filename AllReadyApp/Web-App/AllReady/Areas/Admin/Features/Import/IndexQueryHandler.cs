using System.Linq;
using AllReady.Areas.Admin.ViewModels.Import;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Import
{
    public class IndexQueryHandler : IRequestHandler<IndexQuery, IndexViewModel>
    {
        private readonly AllReadyContext context;

        public IndexQueryHandler(AllReadyContext context)
        {
            this.context = context;
        }

        public IndexViewModel Handle(IndexQuery message)
        {
            var events = context.Events as IQueryable<Event>;
            if (message.OrganizationId != null)
            {
                events = events.Where(x => x.Campaign.ManagingOrganization.Id == message.OrganizationId.Value);
            }

            var eventsWithCampaign = events.Include(e => e.Campaign);

            var viewModel = new IndexViewModel
            {
                Events = eventsWithCampaign.Select(@event => new SelectListItem
                {
                    Value = @event.Id.ToString(),
                    Text = $"{@event.Campaign.ManagingOrganization.Name} > {@event.Campaign.Name} > {@event.Name}"
                }).ToList()
            };

            return viewModel;
        }
    }
}
