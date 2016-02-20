using AllReady.Models;
using System;

namespace AllReady.UnitTest.Features.Organizations
{
    public static class OrganizationHandlerTestHelper
    {
        public static void LoadOrganizationHandlerTestData(AllReadyContext context)
        {
            var validPrivacyPolicy = "Line 1" + Environment.NewLine + "Line 2";
            var htmlPrivacyPolicy = "Line 1" + Environment.NewLine + "<p>Line 2</p>";

            // Organizations
            context.Organizations.Add(new Organization { Id = 1, Name = "Org 1" });
            context.Organizations.Add(new Organization { Id = 2, Name = "Org 2", PrivacyPolicy = validPrivacyPolicy });
            context.Organizations.Add(new Organization { Id = 3, Name = "Org 3", PrivacyPolicy = htmlPrivacyPolicy });

            // Campaigns
            context.Campaigns.Add(new Campaign { Name = "Campaign 1", ManagingOrganizationId = 1 });
            context.Campaigns.Add(new Campaign { Name = "Campaign 2", ManagingOrganizationId = 1 });
            context.Campaigns.Add(new Campaign { Name = "Locked Campaign", ManagingOrganizationId = 1, Locked = true });
            context.Campaigns.Add(new Campaign { Name = "Unlocked Campaign", ManagingOrganizationId = 1, Locked = false });

            context.SaveChanges();
        }
    }
}