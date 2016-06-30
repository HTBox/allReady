using System.Collections.Generic;

namespace AllReady.Areas.Admin.Models.Validators
{
    public class OrganizationEditModelValidator : IOrganizationEditModelValidator
    {
        public List<KeyValuePair<string, string>> Validate(OrganizationEditModel model)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (string.IsNullOrEmpty(model.PrivacyPolicy) && string.IsNullOrEmpty(model.PrivacyPolicyUrl))
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.PrivacyPolicyUrl), "You must either supply a URL to a privacy policy or supply the privacy policy content."));
            }

            return result;
        }
    }

    public interface IOrganizationEditModelValidator
    {
        List<KeyValuePair<string, string>> Validate(OrganizationEditModel model);
    }
}
