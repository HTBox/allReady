using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Goals;
using AllReady.Areas.Admin.ViewModels.Goal;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Goals
{
    public class EditGoalCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task UpdatingExistingGoal()
        {
            // Arrange
            var handler = new GoalEditCommandHandler(Context);
            var newGoal = new GoalEditViewModel { Id = 2, GoalType = GoalType.Numeric, CurrentGoalLevel= 123, NumericGoal = 456, CampaignId = 98, TextualGoal = "We aim to please", Display = true};

            // Act
            var result = await handler.Handle(new GoalEditCommand { Goal = newGoal });
            var savedGoal = Context.CampaignGoals.SingleOrDefault(s => s.Id == 2);

            // Assert
            Assert.Equal(7, Context.CampaignGoals.Count());
            Assert.Equal(2, result);
            Assert.Equal("We aim to please", savedGoal.TextualGoal);
        }

        [Fact]
        public async Task AddNewGoal()
        {
            // Arrange
            var handler = new GoalEditCommandHandler(Context);
            var newGoal = new GoalEditViewModel { GoalType = GoalType.Numeric, CurrentGoalLevel = 123, NumericGoal = 456, CampaignId = 98, TextualGoal = "We aim to please", Display = true };


            // Act
            var result = await handler.Handle(new GoalEditCommand { Goal = newGoal });

            // Assert
            Assert.Equal(8, Context.CampaignGoals.Count());
            Assert.Equal(8, result);
        }

        protected override void LoadTestData()
        {
            GoalsHandlerTestHelper.LoadGoalsHandlerTestData(Context);
        }
    }
}
