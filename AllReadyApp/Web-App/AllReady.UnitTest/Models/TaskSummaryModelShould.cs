using System.Globalization;
using System.Linq;

using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.UnitTest.Extensions;
using Xunit;

namespace AllReady.UnitTest.Models
{
    public class TaskSummaryModelShould
    {
        public class Name
        {
            [Fact]
            public void ValidationShouldFail_IfNoNameIsSupplied()
            {
                //arrange
                using (new CultureContext(new CultureInfo("en")))
                {
                    var model = new TaskSummaryViewModel();
                    var tester = new PropertyValidationHelper<TaskSummaryViewModel>(model);

                    //act
                    var result = tester.ValidateProperty(m => m.Name);

                    //assert
                    Assert.Contains("The Name field is required.", result.Select(r => r.ErrorMessage));
                }
            }

            [Fact]
            public void ValidationShouldSucceed_IfNameIsSupplied()
            {
                //arrange
                using (new CultureContext(new CultureInfo("en")))
                {
                    var model = new TaskSummaryViewModel
                    {
                        Name = "name"
                    };
                    var tester = new PropertyValidationHelper<TaskSummaryViewModel>(model);

                    //act
                    var result = tester.ValidateProperty(m => m.Name);

                    //assert
                    Assert.Empty(result);
                }
            }
        }

        public class NumberOfVolunteersRequired
        {
            [Fact]
            public void ValidationShouldFail_IfNoNumberOfVolunteersRequiredIsSupplied()
            {
                //arrange
                using (new CultureContext(new CultureInfo("en")))
                {
                    var model = new TaskSummaryViewModel();
                    var tester = new PropertyValidationHelper<TaskSummaryViewModel>(model);

                    //act
                    var result = tester.ValidateProperty(m => m.NumberOfVolunteersRequired);

                    //assert
                    Assert.Contains("'Volunteers Required' must be greater than 0", result.Select(r => r.ErrorMessage));
                }
            }

            [Fact]
            public void ValidationShouldFail_IfNumberOfVolunteersRequiredIs0()
            {
                //arrange
                using (new CultureContext(new CultureInfo("en")))
                {
                    var model = new TaskSummaryViewModel
                    {
                        NumberOfVolunteersRequired = 0
                    };
                    var tester = new PropertyValidationHelper<TaskSummaryViewModel>(model);

                    //act
                    var result = tester.ValidateProperty(m => m.NumberOfVolunteersRequired);

                    //assert
                    Assert.Contains("'Volunteers Required' must be greater than 0", result.Select(r => r.ErrorMessage));
                }
            }

            [Fact]
            public void ValidationShouldSucceed_IfNumberOfVolunteersRequiredIs1()
            {
                //arrange
                using (new CultureContext(new CultureInfo("en")))
                {
                    var model = new TaskSummaryViewModel
                    {
                        NumberOfVolunteersRequired = 1
                    };
                    var tester = new PropertyValidationHelper<TaskSummaryViewModel>(model);

                    //act
                    var result = tester.ValidateProperty(m => m.NumberOfVolunteersRequired);

                    //assert
                    Assert.Empty(result);
                }
            }

            [Fact]
            public void ValidationShouldSucceed_IfNumberOfVolunteersRequiredIs100()
            {
                //arrange
                using (new CultureContext(new CultureInfo("en")))
                {
                    var model = new TaskSummaryViewModel
                    {
                        NumberOfVolunteersRequired = 100
                    };
                    var tester = new PropertyValidationHelper<TaskSummaryViewModel>(model);

                    //act
                    var result = tester.ValidateProperty(m => m.NumberOfVolunteersRequired);

                    //assert
                    Assert.Empty(result);
                }
            }

            [Fact]
            public void ValidationShouldSucceed_IfNumberOfVolunteersRequiredIsIntMaxValue()
            {
                //arrange
                using (new CultureContext(new CultureInfo("en")))
                {
                    var model = new TaskSummaryViewModel
                    {
                        NumberOfVolunteersRequired = int.MaxValue
                    };
                    var tester = new PropertyValidationHelper<TaskSummaryViewModel>(model);

                    //act
                    var result = tester.ValidateProperty(m => m.NumberOfVolunteersRequired);

                    //assert
                    Assert.Empty(result);
                }
            }
        }
    }
}
