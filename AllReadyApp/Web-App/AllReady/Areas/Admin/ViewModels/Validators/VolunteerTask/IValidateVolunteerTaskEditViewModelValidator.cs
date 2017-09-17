using System.Collections.Generic;
using System.Threading.Tasks;

using AllReady.Areas.Admin.ViewModels.VolunteerTask;

namespace AllReady.Areas.Admin.ViewModels.Validators.VolunteerTask
{
    public interface IValidateVolunteerTaskEditViewModelValidator
    {
        Task<List<KeyValuePair<string, string>>> Validate(EditViewModel viewModel);
    }
}