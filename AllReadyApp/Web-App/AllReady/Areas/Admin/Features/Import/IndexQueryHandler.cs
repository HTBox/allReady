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
            var viewModel = new IndexViewModel
            {
                //TODO mgmccarthy: pending confirmation of filtering Event's by org for a SiteAdmin or allowing access to all Events
                Events = context.Events.Include(e => e.Campaign)
                .Select(@event => new SelectListItem
                {
                    Value = @event.Id.ToString(),
                    Text = $"{@event.Campaign.ManagingOrganization.Name} > {@event.Campaign.Name} > {@event.Name}"
                }).ToList()
            };

            return viewModel;
        }
    }
}
