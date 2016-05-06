using AllReady.Models;
using System;
using Xunit;

namespace AllReady.UnitTest.DataModels
{
    public class AllReadyTaskTests
    {
        [Fact]
        public void IsClosed_ShouldBeTrue_IfEndDatePriorToCurrentDate()
        {
            var sut = new AllReadyTask();

            sut.EndDateTime = DateTime.UtcNow.AddDays(-1);

            Assert.True(sut.IsClosed);
        }

        [Fact]
        public void IsClosed_ShouldBeFalse_IfEndDateLaterThanCurrentDate()
        {
            var sut = new AllReadyTask();

            sut.EndDateTime = DateTime.UtcNow.AddDays(1);

            Assert.False(sut.IsClosed);
        }

        [Fact]
        public void IsClosed_ShouldBeFalse_IfEndDateIsNull()
        {
            var sut = new AllReadyTask();

            Assert.False(sut.IsClosed);
        }
    }
}
