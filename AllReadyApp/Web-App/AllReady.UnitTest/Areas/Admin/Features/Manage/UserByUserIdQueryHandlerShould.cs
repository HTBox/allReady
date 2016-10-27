using AllReady.Features.Manage;
using AllReady.Models;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Manage
{
    public class UserByUserIdQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task UserByUserIdQueryHandlerInvokesGetUserWithTheCorrectUserId()
        {
            var options = CreateNewContextOptions();

            const string userId = "1";
            var message = new UserByUserIdQuery { UserId = userId };

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(new ApplicationUser
                {
                    Id = userId
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new UserByUserIdQueryHandler(context);
                var user = await sut.Handle(message);

                Assert.Equal(user.Id, userId);
            }
        }
    }
}
