using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Validators
{
    public class EventEditModelValidator : IValidateEventDetailModels
    {
        public List<KeyValuePair<string, string>> Validate(EventEditModel model, CampaignSummaryModel parentCampaign)
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

            if ( model.EventType == EventType.Rally && string.IsNullOrEmpty(model.Location.Address1) )
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.Location.Address1), "Address1 field is required. "));
            }

            return result;
        }
    }

    public interface IValidateEventDetailModels
    {
        List<KeyValuePair<string, string>> Validate(EventEditModel model, CampaignSummaryModel parentCampaign);
    }
}