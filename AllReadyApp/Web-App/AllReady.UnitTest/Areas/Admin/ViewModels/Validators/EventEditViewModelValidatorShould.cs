using System;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Areas.Admin.ViewModels.Validators;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.ViewModels.Validators
{
    public class EventEditViewModelValidatorShould
    {
        [Fact]
        public void ReturnCorrectErrorWhenEndDateTimeIsLessThanStartDateTime()
        {
            var validator = new EventEditViewModelValidator();
            var parentCampaign = new CampaignSummaryViewModel { EndDate = new DateTimeOffset(new DateTime(1999, 2, 1)) };
            var model = new EventEditViewModel
            {
                StartDateTime = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDateTime = new DateTimeOffset(new DateTime(1999, 1, 1))
            };

            var errors = validator.Validate(model, parentCampaign);

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal("End date cannot be earlier than the start date", errors.Find(x => x.Key == "EndDateTime").Value);
        }

        [Fact]
        public void ReturnCorrectErrorWhenModelsStartDateTimeIsLessThanParentCampaignsStartDate()
        {
            var validator = new EventEditViewModelValidator();
            var parentCampaign = new CampaignSummaryViewModel
            {
                StartDate = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(2001, 2, 1))
            };

            var model = new EventEditViewModel
            {
                EndDateTime = new DateTimeOffset(new DateTime(2001, 1, 1))
            };

            var errors = validator.Validate(model, parentCampaign);

            Assert.True(errors.Exists(x => x.Key.Equals("StartDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "StartDateTime").Value, "Start date cannot be earlier than the campaign start date " + parentCampaign.StartDate.ToString("d"));
        }

        [Fact]
        public void RetrunCorrectErrorWhenModelsEndDateTimeIsGreaterThanParentCampaignsEndDate()
        {
            var validator = new EventEditViewModelValidator();
            var parentCampaign = new CampaignSummaryViewModel
            {
                StartDate = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(2001, 1, 1))
            };
            var model = new EventEditViewModel
            {
                StartDateTime = new DateTimeOffset(new DateTime(2001, 1, 1)),
                EndDateTime = new DateTimeOffset(new DateTime(2001, 2, 1)),
            };

            var errors = validator.Validate(model, parentCampaign);

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "End date cannot be later than the campaign end date " + parentCampaign.EndDate.ToString("d"));
        }
    }
}