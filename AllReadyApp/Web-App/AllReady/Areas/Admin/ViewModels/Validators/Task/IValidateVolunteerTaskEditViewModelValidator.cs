using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;

namespace AllReady.Areas.Admin.ViewModels.Validators.Task
{
    public interface IValidateVolunteerTaskEditViewModelValidator
    {
        Task<List<KeyValuePair<string, string>>> Validate(EditViewModel viewModel);
    }
}