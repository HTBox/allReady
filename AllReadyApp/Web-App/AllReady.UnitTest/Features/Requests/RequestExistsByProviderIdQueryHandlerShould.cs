using System.Threading.Tasks;
using AllReady.Features.Requests;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Features.Requests
{
    public class RequestExistsByProviderIdQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnsTrueWhenRequestExists()
        {
            const string providerRequestId = "ProviderId";

            var request = new Request { ProviderRequestId = providerRequestId };
            Context.Requests.Add(request);
            Context.SaveChanges();

            var message = new RequestExistsByProviderIdQuery { ProviderRequestId = providerRequestId };

            var sut = new RequestExistsByProviderIdQueryHandler(Context);
            var result = await sut.Handle(message);

            Assert.True(result);
        }

        [Fact]
        public async Task ReturnsFalseWhenRequestDoesNotExist()
        {
            const string providerRequestId = "AnotherProviderId";

            var message = new RequestExistsByProviderIdQuery { ProviderRequestId = providerRequestId };

            var sut = new RequestExistsByProviderIdQueryHandler(Context);
            var result = await sut.Handle(message);

            Assert.False(result);
        }
    }
}