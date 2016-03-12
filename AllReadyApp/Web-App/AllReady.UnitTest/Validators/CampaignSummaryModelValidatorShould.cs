using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using AllReady.Areas.Admin.Models.Validators;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Validators
{
    public class CampaignSummaryModelValidatorShould
    {
        [Fact]
        public async Task ReportErrorsWhenDatesAreInvalid()
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
            var errors = await validator.Validate(model);

            // assert
            Assert.True(errors.ContainsKey("EndDate"));
        }


    }
}
