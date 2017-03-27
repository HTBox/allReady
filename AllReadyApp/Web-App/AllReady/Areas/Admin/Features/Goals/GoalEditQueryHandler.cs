using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Goal;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Goals
{
    public class GoalEditQueryHandler : IAsyncRequestHandler<GoalEditQuery, GoalEditViewModel>
    {
        private readonly AllReadyContext _context;

        public GoalEditQueryHandler(AllReadyContext context)
        {
            _context = context;

        }

        public async Task<GoalEditViewModel> Handle(GoalEditQuery message)
        {
            GoalEditViewModel result = null;

            var goal = await _context.CampaignGoals
                .AsNoTracking()
                .Include(g => g.Campaign).ThenInclude(c=>c.ManagingOrganization)
                .SingleOrDefaultAsync(c => c.Id == message.GoalId);

            if (goal != null)
            {
                result = new GoalEditViewModel
                {
                    Id = goal.Id,
                    GoalType = goal.GoalType,
                    TextualGoal = goal.TextualGoal,
                    Display = goal.Display,
                    CurrentGoalLevel = goal.CurrentGoalLevel,
                    CampaignId = goal.CampaignId,
                    OrganizationId = goal.Campaign.ManagingOrganizationId,
                    NumericGoal = goal.NumericGoal,
                    CampaignName = goal.Campaign.Name
                };
            }

            return result;
        }
    }
}