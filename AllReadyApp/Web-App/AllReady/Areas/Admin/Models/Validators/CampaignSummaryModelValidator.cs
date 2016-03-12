using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Models.Validators
{
    public class CampaignSummaryModelValidator
    {
        private IMediator _mediator;

        public CampaignSummaryModelValidator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Dictionary<string, string>> Validate (CampaignSummaryModel model)
        {
            var result = new Dictionary<string, string>();

            if (model.EndDate < model.StartDate)
            {
                result.Add(nameof(model.EndDate), "The end date must fall after the start date.");
            }

            return result;
        }
    }
}
