using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Goals
{
    public class GoalDeleteCommandHandler : AsyncRequestHandler<GoalDeleteCommand>
    {
        private readonly AllReadyContext _context;

        public GoalDeleteCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(GoalDeleteCommand message)
        {
            var goal = await _context.CampaignGoals.SingleOrDefaultAsync(s => s.Id == message.GoalId);

            if (goal != null)
            {
                _context.CampaignGoals.Remove(goal);
                await _context.SaveChangesAsync();
            }
        }
    }
}
