using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Extensions;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Extensions
{
    public class EnumerableExtensionsShould
    {
        [Fact]
        public void GroupIntoShouldReturnEmptyEnumerable_WhenInputIsEmpty()
        {
            var input = Enumerable.Empty<string>();

            var result = input.GroupInto(2);

            result.ShouldBeEmpty();
        }

        [Fact]
        public void GroupIntoShouldReturnSingleEnumerable_WhenInputHasOneElement()
        {
            var input = Enumerable.Range(1, 1);

            var result = input.GroupInto(2).ToList();

            result.ShouldHaveSingleItem();
            result[0].ShouldHaveSingleItem();
        }

        [Fact]
        public void GroupIntoShouldReturnGroupsOfTwo_WhenCountIsTwo()
        {
            var input = Enumerable.Range(1, 10);

            var result = input.GroupInto(2).ToList();

            result.Count.ShouldBe(5);
            result.ShouldAllBe(x => x.Count() == 2);
        }

        [Fact]
        public void GroupIntoShouldThrowArgumentNullException_WhenInputIsNull()
        {
            IEnumerable<int> input = null;

            var exception = Should.Throw<ArgumentNullException>(() => input.GroupInto(2).ToList());
            exception.ParamName.ShouldNotBeNullOrEmpty();
        }
    }
}
