using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Api.Data;
using AllReady.Api.Features.Commands;
using AllReady.Api.Features.Queries;
using AllReady.Api.Models.Input;
using AllReady.Api.Models.Output.Campaigns;
using AllReady.Api.Models.Output.Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NodaTime;

namespace AllReady.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IMediator _mediator;

        public CampaignsController(LinkGenerator linkGenerator, IMediator mediator)
        {
            _linkGenerator = linkGenerator;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampaignListerOutputModel>>> Get()
        {
            var campaigns = await _mediator.Send(new ActiveCampaignsListerQuery { });

            foreach(var campaign in campaigns)
            {
                campaign.Link = _linkGenerator.GetPathByAction(
                    HttpContext,
                    "Get",
                    values: new { id = campaign.Id });
            }

            return Ok(campaigns);
        }

        [HttpGet, Route("{id}")]
        public ActionResult<CampaignOutputModel> Get(string id)
        {
            // todo - call a service to load this from a data store using the ID

            var campaign = new CampaignOutputModel
            {
                Id = "8e4ee86d-7636-453a-89f2-6dab38279e66",
                Name = "Charity Campaign 1",
                ShortDescription = "This is a charity campaign!",
                FullDesription = "This is even longer, rambling description of the campaign, which most users will not bother to read!",
                StartDateTime = new LocalDate(2019, 01, 01),
                EndDateTime = new LocalDate(2019, 06, 30),
                TimeZone = DateTimeZoneProviders.Tzdb["Europe/London"],
                ImageUrl = "https://picsum.photos/200/400/?random+8e4ee86d-7636-453a-89f2-6dab38279e66",
                Events = new List<EventListerOutputModel>
                {
                    new EventListerOutputModel
                    {
                        Id = "052fa3e1-f8cd-4f0e-b5f2-3459b6f98c04",
                        Name = "Charity Event 1",
                        ShortDescription = "Marketing activities",
                        StartDateTime = new LocalDate(2019, 01, 01),
                        EndDateTime = new LocalDate(2019, 01, 31),
                        TimeZone = DateTimeZoneProviders.Tzdb["Europe/London"],
                        Link = _linkGenerator.GetPathByAction(
                            HttpContext,
                            "Get",
                            values: new { id = 1 })
                    }
                }
            };

            return campaign;
        }

        [HttpPost, Route("Create")]
        public async Task<ActionResult<CreateCommandResultOutputModel>> Post(CreateCampaignInputModel model)
        {
            // todo validation

            var id = Guid.NewGuid();

            var inputStartDate = model.StartDate.Value; // Model binding will have validated the presence of a date
            var inputEndDate = model.EndDate.Value; // Model binding will have validated the presence of a date

            var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(model.TimeZoneId);

            if (timeZone == null)
                return BadRequest(); // todo RFC7807

            var campaign = new Campaign
            {
                Id = id,
                Name = model.Name,
                StartDateTime = new LocalDate(inputStartDate.Year, inputStartDate.Month, inputStartDate.Day),
                EndDateTime = new LocalDate(inputEndDate.Year, inputEndDate.Month, inputEndDate.Day),
                ImageUrl = model.ImageUrl,
                TimeZone = timeZone,
                ShortDescription = model.ShortDescription,
                FullDescription = model.FullDescription
            };

            var command = new CreateCampaignCommand(campaign);

            await _mediator.Send(command); // todo - handle exceptions

            var uri = _linkGenerator.GetPathByAction(
                            HttpContext,
                            "Get",
                            values: new { id });

            return Ok(new CreateCommandResultOutputModel { DataUri = uri });
        }
    }
}
