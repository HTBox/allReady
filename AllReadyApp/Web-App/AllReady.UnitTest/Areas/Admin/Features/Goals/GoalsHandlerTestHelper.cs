using System.Linq;

using AllReady.Models;

namespace AllReady.UnitTest.Areas.Admin.Features.Goals
{
    public class GoalsHandlerTestHelper
    {
        public static void LoadGoalsHandlerTestData(AllReadyContext context)
        {
            // Organizations
            context.Organizations.Add(new Organization { Id = 1, Name = "Org 1" });
            context.Organizations.Add(new Organization { Id = 2, Name = "Org 2" });

            // Campaigns
            context.Campaigns.Add(new Campaign { Id = 1, Name = "Campaign 1", ManagingOrganizationId = 1 });
            context.Campaigns.Add(new Campaign { Id = 2, Name = "Campaign 2", ManagingOrganizationId = 2 });

            // Goals
            context.CampaignGoals.Add(new CampaignGoal { TextualGoal = "Goal 1" });
            context.CampaignGoals.Add(new CampaignGoal { TextualGoal = "Goal 2" });
            context.CampaignGoals.Add(new CampaignGoal { TextualGoal = "Goal 3" });

            context.CampaignGoals.Add(new CampaignGoal { TextualGoal = "Goal 4" });
            context.CampaignGoals.Add(new CampaignGoal { TextualGoal = "Goal 5" });
            context.CampaignGoals.Add(new CampaignGoal { TextualGoal = "Goal 6" });

            context.CampaignGoals.Add(new CampaignGoal { TextualGoal = "Goal 7" });

            context.SaveChanges();
        }
    }
}