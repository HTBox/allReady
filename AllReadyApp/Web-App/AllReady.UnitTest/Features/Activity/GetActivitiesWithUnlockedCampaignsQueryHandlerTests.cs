using System.Collections.Generic;
using AllReady.Features.Activity;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Activity
{
    public class GetActivitiesWithUnlockedCampaignsQueryHandlerTests
    {
        [Fact]
        public void HandleReturnsActivitiesWitUnlockedCampaigns()
        {
            var activities = new List<Models.Activity>
            {
                new Models.Activity { Id = 1, Campaign = new Campaign { Locked = false, ManagingOrganization = new Organization() }},
                new Models.Activity { Id = 2, Campaign = new Campaign { Locked = true, ManagingOrganization = new Organization() }}
            };

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.Activities).Returns(activities);

            var sut = new GetActivitiesWithUnlockedCampaignsQueryHandler(dataAccess.Object);
            var results = sut.Handle(new GetActivitiesWithUnlockedCampaignsQuery());

            Assert.Equal(activities[0].Id, results[0].Id);
        }
    }
}
