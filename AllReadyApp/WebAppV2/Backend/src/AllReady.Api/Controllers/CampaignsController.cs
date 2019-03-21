using System.Collections.Generic;
using AllReady.Api.Models.Output.Campaigns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace AllReady.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;

        public CampaignsController(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CampaignListerViewModel>> Get()
        {
            // todo - call a service to load this from a data store

            var campaigns = new List<CampaignListerViewModel>
            {
                new CampaignListerViewModel
                {
                    Id = 1,
                    Name = "Charity Campaign 1",
                    ShortDescription = "This is a charity campaign!",
                    StartDateTime = new LocalDate(2019, 01, 01),
                    EndDateTime = new LocalDate(2019, 06, 30),
                    TimeZone = DateTimeZoneProviders.Tzdb["Europe/London"],
                    Hidden = false,
                    DatUri = _linkGenerator.GetPathByAction(
                        HttpContext,
                        "Get",
                        values: new { id = 1 })
                },
                new CampaignListerViewModel
                {
                    Id = 2,
                    Name = "Charity Campaign 2",
                    ShortDescription = "This is another charity campaign!",
                    StartDateTime = new LocalDate(2019, 04, 01),
                    EndDateTime = new LocalDate(2019, 05, 31),
                    TimeZone = DateTimeZoneProviders.Tzdb["Europe/London"],
                    Hidden = false,
                    DatUri = _linkGenerator.GetPathByAction(
                        HttpContext,
                        "Get",
                        values: new { id = 2 })
                }
            };

            return campaigns;
        }

        [HttpGet, Route("{id}")]
        public ActionResult<CampaignViewModel> Get(int id)
        {
            // todo - call a service to load this from a data store using the ID

            var campaign = new CampaignViewModel
            {
                Id = 1,
                Name = "Charity Campaign 1",
                ShortDescription = "This is a charity campaign!",
                FullDesription = "This is even longer, rambling description of the campaign, which most users will not bother to read!",
                StartDateTime = new LocalDate(2019, 01, 01),
                EndDateTime = new LocalDate(2019, 06, 30),
                TimeZone = DateTimeZoneProviders.Tzdb["Europe/London"],
                Hidden = false
            };

            return campaign;
        }
    }
}
