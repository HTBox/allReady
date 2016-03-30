using MediatR;
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
                result.Add(nameof(model.EndDate), "The end date must fall on or after the start date.");
            }

            var postalCodeValidation = new LocationEditModelValidator(_mediator);
            var postalCodeErrors = await postalCodeValidation.Validate(model.Location);
            postalCodeErrors.ToList().ForEach(e => result.Add(e.Key, e.Value));

            return result;
        }
    }
}
