using System.Linq;
using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class GetMyEventsQueryHandlerTests
    {
        [Fact]
        public void ReturnsExpectedEvents()
        {
            var eventSignups = new[]
            {
                new EventSignup
                {
                    Event = new Models.Event
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
                new EventSignup
                {
                    Event = new Models.Event
                    {
                        Name = "Expected Event",
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

            var command = new GetMyEventsQuery {UserId = "B62AF756-809D-40B7-AA88-237E52889C45"};
            mockDbAccess.Setup(db => db.GetEventSignups(command.UserId)).Returns(eventSignups);

            var sut = new GetMyEventsQueryHandler(mockDbAccess.Object);
            var response = sut.Handle(command);

            response.ShouldSatisfyAllConditions(
                () => response.Items.Count.ShouldBe(1),
                () => response.Items.First().Title.ShouldBe("Expected Event")
            );
        }
    }
}
