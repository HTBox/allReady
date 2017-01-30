using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.ViewModels.UnlinkedRequests;

namespace AllReady.Areas.Admin.ViewModels.Validators
{
    public interface IUnlinkedRequestViewModelValidator
    {
        List<KeyValuePair<string, string>> Validate(UnlinkedRequestViewModel model);
    }

    public class UnlinkedRequestViewModelValidator : IUnlinkedRequestViewModelValidator
    {
        public List<KeyValuePair<string, string>> Validate(UnlinkedRequestViewModel model)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (model.EventId < 1)
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.EventId), "You must select an event"));
            }
            if (!model.Requests.Any(req => req.IsSelected))
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.Requests), "You must select at least one request"));
            }
            return result;
        }
    }
}
