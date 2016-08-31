using AllReady.Features.Manage;
using AllReady.Models;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Manage
{
    public class UserByUserIdQueryHandlerTests : InMemoryContextTest
    {
        [Fact]
        public async Task UserByUserIdQueryHandlerInvokesGetUserWithTheCorrectUserId()
        {
            var options = this.CreateNewContextOptions();

            const string userId = "1";
            var message = new UserByUserIdQuery { UserId = userId };

            using (var context = new AllReadyContext(options)) {
                context.Users.Add(new ApplicationUser {
                    Id = userId
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new UserByUserIdQueryHandler(context);
                var user = sut.Handle(message);

                Assert.Equal(user.Id, userId);
            }
        }
    }
}
