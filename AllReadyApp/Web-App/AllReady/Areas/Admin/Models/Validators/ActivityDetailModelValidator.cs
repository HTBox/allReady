using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Models.Validators
{
    public class ActivityDetailModelValidator
    {
        private IMediator _mediator;

        public ActivityDetailModelValidator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Dictionary<string, string>> Validate(ActivityDetailModel model, CampaignSummaryModel parentCampaign)
        {
            var result = new Dictionary<string, string>();

            if (model.EndDateTime < model.StartDateTime)
            {
                result.Add(nameof(model.EndDateTime), "End date cannot be earlier than the start date");
            }

            if (model.StartDateTime < parentCampaign.StartDate)
            {
                result.Add(nameof(model.StartDateTime), "Start date cannot be earlier than the campaign start date " + parentCampaign.StartDate.ToString("d"));
            }

            if (model.EndDateTime > parentCampaign.EndDate)
            {
                result.Add(nameof(model.EndDateTime), "End date cannot be later than the campaign end date " + parentCampaign.EndDate.ToString("d"));
            }

            var postalCodeValidation = new LocationEditModelValidator(_mediator);
            var postalCodeErrors = await postalCodeValidation.Validate(model.Location);
            postalCodeErrors.ToList().ForEach(e => result.Add(e.Key, e.Value));
            return result;
        }
    }
}
