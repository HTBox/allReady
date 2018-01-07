using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.ViewModels.Home;
using AllReady.Features.Campaigns;
using AllReady.Features.Home;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllReady.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Details for the currently featured campaign (if one is set)
        /// </summary>
        public CampaignSummaryViewModel FeaturedCampaign { get; private set; }

        /// <summary>
        /// A list of the currently active  and upcoming events
        /// </summary>
        public IEnumerable<ActiveOrUpcomingEvent> ActiveOrUpcomingEvents { get; private set; }

        /// <summary>
        /// Identifies whether the model includes a featured campaign to be shown
        /// </summary>
        public bool HasFeaturedCampaign => FeaturedCampaign != null;

        public async Task OnGetAsync()
        {
            // These can start and run in parallel since neither is dependent on the other
            var featuredCampaignQueryTask = _mediator.SendAsync(new FeaturedCampaignQuery());
            var activeOrUpcomingEventsQueryTask = _mediator.SendAsync(new ActiveOrUpcomingEventsQuery());

            await Task.WhenAll(featuredCampaignQueryTask, activeOrUpcomingEventsQueryTask);

            FeaturedCampaign = await featuredCampaignQueryTask;
            ActiveOrUpcomingEvents = await activeOrUpcomingEventsQueryTask;
        }
    }
}
