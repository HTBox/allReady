using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Manage;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Manage
{
    public class UpdateUserHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task UpdateUserHandlerInvokesUpdateUserWithTheCorrectUser()
        {
            var options = this.CreateNewContextOptions();

            const string userId = "12345";
            const string firstName = "changed";
            var message = new UpdateUser { User = new ApplicationUser {
                Id = userId,
                FirstName = firstName
            } };

            using (var context = new AllReadyContext(options)) {
                context.Users.Add(new ApplicationUser {
                    Id = userId,
                    FirstName = "notChanged"
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new UpdateUserHandler(context);
                await sut.Handle(message);
            }

            using (var context = new AllReadyContext(options)) {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                Assert.NotNull(user);
                Assert.Equal(user.FirstName, firstName);
            }

        }
    }
}
