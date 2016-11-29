using AllReady.Features.Requests;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Features.Requests
{
    public class RequestExistsByProviderIdQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public void ReturnsTrueWhenRequestExists()
        {
            const string providerRequestId = "ProviderId";

            var request = new Request { ProviderId = providerRequestId };
            Context.Requests.Add(request);
            Context.SaveChanges();

            var message = new RequestExistsByProviderIdQuery { ProviderRequestId = providerRequestId };

            var sut = new RequestExistsByProviderIdQueryHandler(Context);
            var result = sut.Handle(message);

            Assert.True(result);
        }

        [Fact]
        public void ReturnsFalseWhenRequestDoesNotExist()
        {
            const string providerRequestId = "AnotherProviderId";

            var message = new RequestExistsByProviderIdQuery { ProviderRequestId = providerRequestId };

            var sut = new RequestExistsByProviderIdQueryHandler(Context);
            var result = sut.Handle(message);

            Assert.False(result);
        }
    }
}
