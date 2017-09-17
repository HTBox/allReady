using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Goals
{
    public class GoalEditCommandHandler : IAsyncRequestHandler<GoalEditCommand, int>
    {
        private readonly AllReadyContext _context;

        public GoalEditCommandHandler(AllReadyContext context)
        {
            _context = context;
        }


        public async Task<int> Handle(GoalEditCommand message)
        {
            var msgGoal = message.Goal;

            var goal = await _context.CampaignGoals.SingleOrDefaultAsync(s => s.Id == msgGoal.Id) ??
                        _context.Add(new CampaignGoal()).Entity;

            goal.CampaignId = msgGoal.CampaignId;
            goal.GoalType = msgGoal.GoalType;
            goal.TextualGoal = msgGoal.TextualGoal;
            goal.NumericGoal = msgGoal.NumericGoal;
            goal.CurrentGoalLevel = msgGoal.CurrentGoalLevel;
            goal.Display = msgGoal.Display;

            await _context.SaveChangesAsync();

            return goal.Id;
        }
    }
}
