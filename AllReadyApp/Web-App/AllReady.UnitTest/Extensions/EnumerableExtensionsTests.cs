using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllReady.UnitTest.Extensions
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void TestOrderByAscending()
        {
            var customers = new List<Customer>
            {
                new Customer { Id = 3 },
                new Customer { Id = 2 },
                new Customer { Id = 1 }
            };

            customers = customers.OrderBy(x => x.Id).ToList();

            Assert.True(customers.IsOrderedByAscending(x => x.Id));
        }

        [Fact]
        public void IsOrderByDescending()
        {
            var customers = new List<Customer>
            {
                new Customer { Id = 3 },
                new Customer { Id = 2 },
                new Customer { Id = 1 }
            };

            Assert.False(customers.IsOrderedByAscending(x => x.Id));
        }

        private class Customer
        {
            public int Id { get; set; }
        }
    }
}
