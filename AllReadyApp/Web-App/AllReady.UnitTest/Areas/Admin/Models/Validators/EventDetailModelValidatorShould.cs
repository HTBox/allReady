using System;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Areas.Admin.Models.Validators;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Models.Validators
{
    public class EventDetailModelValidatorShould
    {
        [Fact]
        public void ReportErrorsWhenEndDateIsInvalid()
        {
            // arrage
            var mediator = new Mock<IMediator>();
            var validator = new EventDetailModelValidator(mediator.Object);
            var campaign = new CampaignSummaryModel();
            var model = new EventDetailModel
            {
                StartDateTime = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDateTime = new DateTimeOffset(new DateTime(1999, 1, 1))
            };

            // act
            var errors = validator.Validate(model, campaign);

            // assert
            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
        }

        [Fact]
        public void ReportErrorsWhenEndDateOutsideCampaignWindow()
        {
            // arrage
            var mediator = new Mock<IMediator>();
            var validator = new EventDetailModelValidator(mediator.Object);
            var campaign = new CampaignSummaryModel()
            {
                StartDate = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(1999, 1, 1))
            };

            var model = new EventDetailModel
            {
                EndDateTime = new DateTimeOffset(new DateTime(2001, 1, 1))
            };

            // act
            var errors = validator.Validate(model, campaign);

            // assert
            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
        }

        [Fact]
        public void ReportErrorsWhenStartDateOutsideCampaignWindow()
        {
            // arrage
            var mediator = new Mock<IMediator>();
            var validator = new EventDetailModelValidator(mediator.Object);
            var campaign = new CampaignSummaryModel()
            {
                StartDate = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(2001, 1, 1))
            };
            var model = new EventDetailModel
            {
                StartDateTime = new DateTimeOffset(new DateTime(1999, 1, 1)),
            };

            // act
            var errors = validator.Validate(model, campaign);

            // assert
            Assert.True(errors.Exists(x => x.Key.Equals("StartDateTime")));
        }

    }
}
