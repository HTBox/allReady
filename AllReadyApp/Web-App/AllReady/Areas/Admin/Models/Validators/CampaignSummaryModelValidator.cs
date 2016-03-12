using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Models.Validators
{
    public class CampaignSummaryModelValidator
    {
        private IMediator _mediator;

        public CampaignSummaryModelValidator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Dictionary<string, string>> Validate (CampaignSummaryModel model)
        {
            var result = new Dictionary<string, string>();

            if (model.EndDate < model.StartDate)
            {
                result.Add(nameof(model.EndDate), "The end date must fall after the start date.");
            }

            if (!string.IsNullOrEmpty(model.Location?.PostalCode))
            {
                bool validPostcode = await _mediator.SendAsync(new CheckValidPostcodeQueryAsync
                {
                    Postcode = new PostalCodeGeo
                    {
                        City = model.Location.City,
                        State = model.Location.State,
                        PostalCode = model.Location.PostalCode
                    }
                });

                if (!validPostcode)
                {
                    result.Add(nameof(model.Location.PostalCode), "The city, state and postal code combination is not valid");
                }
            }

            return result;
        }
    }
}
