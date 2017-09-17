using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Goals;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Goals
{
    public class GoalDeleteQueryHandlerShould : InMemoryContextTest
    {
        private const int CampaignId = 123;
        private const int OrgId = 123;

        protected override void LoadTestData()
        {
            var org = new Organization {Id = OrgId};
            Context.Organizations.Add(org);

            Context.CampaignGoals.Add(new CampaignGoal
            {
                Id = 4,
                GoalType = GoalType.Numeric,
                TextualGoal = "Goal 4",
                NumericGoal = 100,
                CampaignId = CampaignId,
                Campaign = new Campaign {Id = CampaignId, ManagingOrganizationId = OrgId, ManagingOrganization = org }
            });
            Context.SaveChanges();
        }

        [Fact]
        public async Task CorrectGoalReturnedWhenIdInMessage()
        {
            var handler = new GoalDeleteQueryHandler(Context);
            var result = await handler.Handle(new GoalDeleteQuery {GoalId = 4});

            Assert.NotNull(result);
            Assert.Equal(4, result.Id);
            Assert.Equal(GoalType.Numeric, result.GoalType);
            Assert.Equal("Goal 4", result.TextualGoal);
            Assert.Equal(100, result.NumericGoal);
            Assert.Equal(OrgId, result.OwningOrganizationId);
            Assert.Equal(CampaignId, result.CampaignId);
        }

        [Fact]
        public async Task NullReturnedWhenGoalIdDoesNotExists()
        {
            var handler = new GoalDeleteQueryHandler(Context);
            var result = await handler.Handle(new GoalDeleteQuery {GoalId = 100});

            Assert.Null(result);
        }

        [Fact]
        public async Task NullReturnedWhenGoalIdNotInMessage()
        {
            var handler = new GoalDeleteQueryHandler(Context);
            var result = await handler.Handle(new GoalDeleteQuery());

            Assert.Null(result);
        }
    }
}
