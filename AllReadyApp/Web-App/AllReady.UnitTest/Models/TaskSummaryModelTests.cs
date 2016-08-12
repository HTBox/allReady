using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AllReady.Areas.Admin.ViewModels.Task;
using Xunit;

namespace AllReady.UnitTest.ViewModels
{
    public class TaskSummaryModelTests
    {
        [Fact]
        public void ValidationShouldFail_IfNoNameIsSupplied()
        {
            var model = new TaskSummaryModel();
            var result = ValidateModel(model);

            var nameError = result.Where(r => r.ErrorMessage == "The Name field is required.").FirstOrDefault();

            Assert.NotNull(nameError);
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}
