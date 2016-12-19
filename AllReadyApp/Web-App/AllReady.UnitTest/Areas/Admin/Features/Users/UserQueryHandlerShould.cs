using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Users;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Users
{
    public class UserQueryHandlerShould : InMemoryContextTest
    {
        private string _userId;

        protected override void LoadTestData()
        {
            var user = new ApplicationUser();

            Context.Users.Add(user);
            Context.SaveChanges();

            _userId = user.Id;
        }

        [Fact]
        public async Task UserExists()
        {
            var query = new UserQuery() { UserId = _userId };
            var handler = new UserQueryHandler(Context);
            var result = await handler.Handle(query);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UserDoesNotExist()
        {
            var query = new UserQuery { UserId = string.Empty };
            var handler = new UserQueryHandler(Context);
            var result = await handler.Handle(query);
            Assert.Null(result);
        }
    }
}
