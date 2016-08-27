using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Site;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Site
{
    public class IndexQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnTheCorrectData()
        {
            var applicationUser = new ApplicationUser { Id = "1", UserName = "UserName", EmailConfirmed = true };

            Context.Add(new ApplicationUser { Id = "1", UserName = "UserName", EmailConfirmed = true });
            Context.SaveChanges();

            var sut = new IndexQueryHandler(Context);
            var result = await sut.Handle(new IndexQuery());

            Assert.Equal(result.Users[0].Id, applicationUser.Id);
            Assert.Equal(result.Users[0].UserName, applicationUser.UserName);
            Assert.Equal(result.Users[0].EmailConfirmed, applicationUser.EmailConfirmed);
        }
    }
}
