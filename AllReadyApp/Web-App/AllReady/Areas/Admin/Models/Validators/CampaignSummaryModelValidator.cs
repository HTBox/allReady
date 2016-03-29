using AllReady.Areas.Admin.Features.Shared;
using AllReady.Models;
using MediatR;
using System.Collections.Generic;
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
                result.Add(nameof(model.EndDate), "The end date must fall on or after the start date.");
            }

            // Temporary code to avoid current database update error when the post code geo does not exist in the database.
            if (!string.IsNullOrEmpty(model.Location?.PostalCode))
            {
                var validPostcode = await _mediator.SendAsync(new CheckValidPostcodeQueryAsync
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