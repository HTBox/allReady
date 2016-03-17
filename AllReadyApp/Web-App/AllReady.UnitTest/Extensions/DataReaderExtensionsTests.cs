using System;
using System.Collections.Generic;
using System.Data;
using Moq;
using Xunit;
using AllReady.Extensions;

namespace AllReady.UnitTest.Extensions
{
    public class DataReaderExtensionsTests
    {
        private int _rowCount = -1;

        [Fact]
        public void DataReaderMapToListReturnsNonNullList()
        {
            var dataReader = GetDataReader(
                new List<DummyInputObject>
                {
                    new DummyInputObject
                    {
                        DummyBool = true,
                        DummyInt = 1
                    },
                }
            );

            var mapped = dataReader.DataReaderMapToList<DummyOutputObject>();
            var expected = new List<DummyOutputObject>
            {
                new DummyOutputObject
                {
                    DummyBool = true,
                    DummyInt = 1
                }
            };
            Assert.Equal(expected, mapped, new DummyOutputObjectComparer());
        }


        [Fact]
        public void DataReaderMapToListReturnsMultipleItems()
        {
            var dataReader = GetDataReader(
                new List<DummyInputObject>
                {
                    new DummyInputObject
                    {
                        DummyBool = true,
                        DummyInt = 1
                    },
                     new DummyInputObject
                    {
                        DummyBool = false,
                        DummyInt = 2
                    }
                }
            );

            var mapped = dataReader.DataReaderMapToList<DummyOutputObject>();
            var expected = new List<DummyOutputObject>
            {
                new DummyOutputObject
                {
                    DummyBool = true,
                    DummyInt = 1
                },
                new DummyOutputObject
                {
                    DummyBool = false,
                    DummyInt = 2
                }
            };
            Assert.Equal(expected, mapped, new DummyOutputObjectComparer());
        }

        [Fact]
        public void DataReaderMapToListReturnsDbNullAssignment()
        {
            var dataReader = GetDataReader(
                new List<DummyInputObject>
                {
                    new DummyInputObject
                    {
                        DummyBool = DBNull.Value,
                        DummyInt =  DBNull.Value
                    },
                }
            );

            var mapped = dataReader.DataReaderMapToList<DummyOutputObject>();
            var expected = new List<DummyOutputObject>
            {
                new DummyOutputObject
                {
                    DummyBool = null,
                    DummyInt = null
                }
            };
            Assert.Equal(expected, mapped, new DummyOutputObjectComparer());
        }

        private IDataReader GetDataReader(IReadOnlyList<DummyInputObject> dummyObjects)
        {
            var dataReaderMock = new Mock<IDataReader>();

            dataReaderMock
                .Setup(mock => mock.Read())
                .Returns(() => 
                _rowCount < dummyObjects.Count - 1)
                .Callback(() => _rowCount++);

            dataReaderMock.Setup(mock => mock["DummyInt"])
                .Returns(() => 
                dummyObjects[_rowCount].DummyInt);
            dataReaderMock.Setup(mock => mock["DummyBool"])
                .Returns(() => 
                dummyObjects[_rowCount].DummyBool);

            return dataReaderMock.Object;
        }
        private class DummyOutputObjectComparer : IEqualityComparer<DummyOutputObject>
        {
            public bool Equals(DummyOutputObject x, DummyOutputObject y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null | y == null)
                    return false;
                return x.DummyInt == y.DummyInt
                    && x.DummyBool == y.DummyBool;
            }

            public int GetHashCode(DummyOutputObject obj)
            {
                // fake a hash code 
                return obj.DummyInt.GetHashCode();
            }
        }

        private class DummyInputObject
        {
            public object DummyInt { get; set; }
            public object DummyBool { get; set; }
        }

        private class DummyOutputObject
        {
            public int? DummyInt { get; set; }
            public bool? DummyBool { get; set; }
        }
    }
}
