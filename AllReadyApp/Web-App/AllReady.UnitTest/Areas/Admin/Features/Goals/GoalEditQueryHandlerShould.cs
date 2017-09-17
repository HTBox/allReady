using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Goals;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Goals
{
    public class GoalEditQueryHandlerShould : InMemoryContextTest
    {
        private const int CampaignId = 123;
        private const int OrgId = 123;

        protected override void LoadTestData()
        {
            var org = new Organization { Id = OrgId };
            Context.Organizations.Add(org);

            Context.CampaignGoals.Add(new CampaignGoal
            {
                Id = 4,
                TextualGoal = "Goal 4",
                GoalType = GoalType.Numeric,
                NumericGoal = 100,
                Display = true,
                CurrentGoalLevel = 25,
                CampaignId = CampaignId,
                Campaign = new Campaign {Id = CampaignId, Name = "Campaign name", ManagingOrganizationId = OrgId, ManagingOrganization = org }
            });
            Context.SaveChanges();
        }

        [Fact]
        public async Task CorrectGoalReturnedWhenIdInMessage()
        {
            var handler = new GoalEditQueryHandler(Context);
            var result = await handler.Handle(new GoalEditQuery {GoalId = 4});

            Assert.NotNull(result);
            Assert.Equal(4, result.Id);
            Assert.Equal(GoalType.Numeric, result.GoalType);
            Assert.Equal("Goal 4", result.TextualGoal);
            Assert.True(result.Display);
            Assert.Equal(100, result.NumericGoal);
            Assert.Equal(25, result.CurrentGoalLevel);
            Assert.Equal(CampaignId, result.CampaignId);
            Assert.Equal("Campaign name", result.CampaignName);
            Assert.Equal(OrgId, result.OrganizationId);
        }

        [Fact]
        public async Task NullReturnedWhenGoalIdDoesNotExists()
        {
            var handler = new GoalEditQueryHandler(Context);
            var result = await handler.Handle(new GoalEditQuery {GoalId = 100});

            Assert.Null(result);
        }

        [Fact]
        public async Task NullReturnedWhenGoalIdNotInMessage()
        {
            var handler = new GoalEditQueryHandler(Context);
            var result = await handler.Handle(new GoalEditQuery());

            Assert.Null(result);
        }
    }
}
