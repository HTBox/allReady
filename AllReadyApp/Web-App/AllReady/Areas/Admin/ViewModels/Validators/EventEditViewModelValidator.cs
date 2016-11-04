using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Validators
{
    public class EventEditViewModelValidator : IValidateEventEditViewModels
    {
        public List<KeyValuePair<string, string>> Validate(EventEditViewModel viewModel, CampaignSummaryViewModel parentCampaign)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (viewModel.EndDateTime < viewModel.StartDateTime)
            {
                result.Add(new KeyValuePair<string, string>(nameof(viewModel.EndDateTime), "End date cannot be earlier than the start date"));
            }

            if (viewModel.StartDateTime < parentCampaign.StartDate)
            {
                result.Add(new KeyValuePair<string, string>(nameof(viewModel.StartDateTime), "Start date cannot be earlier than the campaign start date " + parentCampaign.StartDate.ToString("d")));
            }

            if (viewModel.EndDateTime > parentCampaign.EndDate)
            {
                result.Add(new KeyValuePair<string, string>(nameof(viewModel.EndDateTime), "End date cannot be later than the campaign end date " + parentCampaign.EndDate.ToString("d")));
            }

            if (viewModel.EventType == EventType.Rally && string.IsNullOrEmpty(viewModel.Location.Address1))
            {
                result.Add(new KeyValuePair<string, string>(nameof(viewModel.Location.Address1), "Address1 field is required. "));
            }

            return result;
        }
    }

    public interface IValidateEventEditViewModels
    {
        List<KeyValuePair<string, string>> Validate(EventEditViewModel viewModel, CampaignSummaryViewModel parentCampaign);
    }
}