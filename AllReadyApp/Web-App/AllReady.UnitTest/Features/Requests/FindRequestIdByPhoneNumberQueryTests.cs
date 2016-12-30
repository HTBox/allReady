using AllReady.Features.Requests;
using Shouldly;
using System;
using Xunit;

namespace AllReady.UnitTest.Features.Requests
{
    public class FindRequestIdByPhoneNumberQueryTests
    {
        private const string GoodPhoneNumber = "0001112222";

        [Fact]
        public void Ctor_ThrowsException_WhenPhoneNumberIsNull()
        {
            Assert.Throws<ArgumentException>(() => new FindRequestIdByPhoneNumberQuery(null));
        }

        [Fact]
        public void Ctor_SetsPhoneNumber()
        {
            var sut = new FindRequestIdByPhoneNumberQuery(GoodPhoneNumber);
            sut.PhoneNumber.ShouldBe(GoodPhoneNumber);
        }
    }
}
