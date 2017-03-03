using System;
using System.Collections;
using System.Collections.Generic;

namespace AllReady.ViewModels.Shared
{
    public class KeywordCollection : IEnumerable<string>
    {
        private const string DELIMITER = "|";
        private static readonly Func<string, string[]> Parse = (input) => input.Split(DELIMITER[0]);
        private readonly List<string> _collection;

        public KeywordCollection() : this(new string[] { }) { }

        public KeywordCollection(string delimited) : this(Parse(delimited)) { }

        public KeywordCollection(string[] source)
        {
            _collection = new List<string>(source);
        }

        public string this[int i] => _collection[i];

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(DELIMITER, _collection.ToArray());
        }
    }
}
