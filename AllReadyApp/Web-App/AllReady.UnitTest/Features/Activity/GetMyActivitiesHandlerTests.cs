using System.Linq;
using AllReady.Features.Activity;
using AllReady.Models;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Features.Activity
{
    public class GetMyActivitiesHandlerTests
    {
        [Fact]
        public void ReturnsExpectedActivities()
        {
            var activitySignups = new[]
            {
                new ActivitySignup
                {
                    Activity = new Models.Activity
                    {
                        Campaign = new Campaign
                        {
                            ManagingOrganization = new Organization
                            {
                                Id = 1,
                                Name = "Some Organization"
                            },
                            Locked = true
                        }
                    }
                },
                new ActivitySignup
                {
                    Activity = new Models.Activity
                    {
                        Name = "Expected Activity",
                        Campaign = new Campaign
                        {
                            ManagingOrganization = new Organization
                            {
                                Id = 1,
                                Name = "Some Organization"
                            },
                            Locked = false
                        }
                    }
                }
            };

            var mockDbAccess = new Mock<IAllReadyDataAccess>();

            var command = new GetMyActivitiesCommand {UserId = "B62AF756-809D-40B7-AA88-237E52889C45"};
            mockDbAccess.Setup(db => db.GetActivitySignups(command.UserId))
                        .Returns(activitySignups);

            var sut = new GetMyActivitiesHandler(mockDbAccess.Object);
            var response = sut.Handle(command);

            response.ShouldSatisfyAllConditions(
                () => response.Items.Count.ShouldBe(1),
                () => response.Items.First().Title.ShouldBe("Expected Activity")
            );
        }
    }
}
