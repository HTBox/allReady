using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Goal;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Goals
{
    public class GoalDeleteQueryHandler : IAsyncRequestHandler<GoalDeleteQuery, GoalDeleteViewModel>
    {
        private readonly AllReadyContext _context;

        public GoalDeleteQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<GoalDeleteViewModel> Handle(GoalDeleteQuery message)
        {
            GoalDeleteViewModel result = null;

            var goal = await _context.CampaignGoals
                .AsNoTracking()
                .Include(g => g.Campaign).ThenInclude(c => c.ManagingOrganization)
                .SingleOrDefaultAsync(c => c.Id == message.GoalId);

            if (goal != null)
            {
                result = new GoalDeleteViewModel
                {
                    Id = goal.Id,
                    GoalType = goal.GoalType,
                    TextualGoal = goal.TextualGoal,
                    NumericGoal = goal.NumericGoal,
                    OwningOrganizationId = goal.Campaign.ManagingOrganizationId,
                    CampaignId = goal.CampaignId
                };
            }

            return result;
        }
    }
}