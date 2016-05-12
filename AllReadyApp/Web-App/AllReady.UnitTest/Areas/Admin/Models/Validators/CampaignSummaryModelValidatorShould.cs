using System;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Shared;
using AllReady.Areas.Admin.Models;
using AllReady.Areas.Admin.Models.Validators;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Models.Validators
{
    public class CampaignSummaryModelValidatorShould
    {
        [Fact]
        public void ReportErrorsWhenDatesAreInvalid()
        {
            // arrage
            var mediator = new Mock<IMediator>();
            var validator = new CampaignSummaryModelValidator(mediator.Object);
            var model = new CampaignSummaryModel
            {
                StartDate = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(1999, 1, 1))
            };

            // act
            var errors = validator.Validate(model);

            // assert
            Assert.True(errors.ContainsKey("EndDate"));
        }

    }
}
