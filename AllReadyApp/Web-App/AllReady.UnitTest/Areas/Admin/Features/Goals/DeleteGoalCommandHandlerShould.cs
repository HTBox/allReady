using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Goals;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Goals
{
    public class DeleteGoalCommandHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            Context.CampaignGoals.Add(new CampaignGoal { Id = 1 });
            Context.SaveChanges();
        }

        [Fact]
        public async Task DeleteAnExistingGoalWithMatchingGoalId()
        {
            var command = new GoalDeleteCommand { GoalId = 1 };
            var handler = new GoalDeleteCommandHandler(Context);
            await handler.Handle(command);

            var result = Context.CampaignGoals.Count();
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task NotDeleteAnExistingGoalWithNonMatchingGoalId()
        {
            var command = new GoalDeleteCommand { GoalId = 2 };
            var handler = new GoalDeleteCommandHandler(Context);
            await handler.Handle(command);

            var result = Context.CampaignGoals.Count();
            Assert.Equal(1, result);
        }
    }
}
