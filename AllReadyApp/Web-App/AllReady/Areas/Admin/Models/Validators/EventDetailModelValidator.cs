using System.Collections.Generic;

namespace AllReady.Areas.Admin.Models.Validators
{
    public class EventDetailModelValidator : IValidateEventDetailModels
    {
        public List<KeyValuePair<string, string>> Validate(EventDetailModel model, CampaignSummaryModel parentCampaign)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (model.EndDateTime < model.StartDateTime)
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.EndDateTime), "End date cannot be earlier than the start date"));
            }

            if (model.StartDateTime < parentCampaign.StartDate)
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.StartDateTime), "Start date cannot be earlier than the campaign start date " + parentCampaign.StartDate.ToString("d")));
            }

            if (model.EndDateTime > parentCampaign.EndDate)
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.EndDateTime), "End date cannot be later than the campaign end date " + parentCampaign.EndDate.ToString("d")));
            }

            return result;
        }
    }

    public interface IValidateEventDetailModels
    {
        List<KeyValuePair<string, string>> Validate(EventDetailModel model, CampaignSummaryModel parentCampaign);
    }
}