using System;
using System.Collections.Generic;
using AllReady.Services.Mapping.Routing;
using Xunit;
using Shouldly;

namespace AllReady.UnitTest.Services.Mapping.Routing
{
    public class OptimizeRouteCriteriaTests
    {
        [Fact]
        public void Ctor_ThrowsArgException_WhenStartAddressIsNull()
        {
            Assert.Throws<ArgumentException>(() => new OptimizeRouteCriteria(null, "end address",
                new List<OptimizeRouteWaypoint> {new OptimizeRouteWaypoint(50, 50, Guid.NewGuid())}));
        }

        [Fact]
        public void Ctor_ThrowsArgException_WhenEndAddressIsNull()
        {
            Assert.Throws<ArgumentException>(() => new OptimizeRouteCriteria("start address", null,
                new List<OptimizeRouteWaypoint> { new OptimizeRouteWaypoint(50, 50, Guid.NewGuid()) }));
        }

        [Fact]
        public void Ctor_ThrowsArgException_WhenWaypointsIsNull()
        {
            Assert.Throws<ArgumentException>(() => new OptimizeRouteCriteria("start address", "end address", null));
        }

        [Fact]
        public void Ctor_ThrowsArgException_WhenWaypointsIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => new OptimizeRouteCriteria("start address", "end address", new List<OptimizeRouteWaypoint>()));
        }

        [Fact]
        public void Ctor_EncodesStartAddress()
        {
            var sut = new OptimizeRouteCriteria("start address", "end address", 
                new List<OptimizeRouteWaypoint> { new OptimizeRouteWaypoint(50, 50, Guid.NewGuid()) });

            sut.StartAddress.ShouldBe("start%20address");
        }

        [Fact]
        public void Ctor_EncodesEndAddress()
        {
            var sut = new OptimizeRouteCriteria("start address", "end address",
                new List<OptimizeRouteWaypoint> { new OptimizeRouteWaypoint(50, 50, Guid.NewGuid()) });

            sut.EndAddress.ShouldBe("end%20address");
        }

        [Fact]
        public void Ctor_SetsWaypoints()
        {
            var sut = new OptimizeRouteCriteria("start address", "end addres",
                new List<OptimizeRouteWaypoint> { new OptimizeRouteWaypoint(50, 50, Guid.NewGuid()) });

            sut.Waypoints.Count.ShouldBe(1);
        }
    }
}
