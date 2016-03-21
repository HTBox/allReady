using System.Collections.Generic;
using Microsoft.AspNet.Mvc.Rendering;

namespace AllReady.UnitTest
{
    public class SelectListItemComparer : IEqualityComparer<SelectListItem>
    {
        public bool Equals(SelectListItem x, SelectListItem y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.Text.Equals(y.Text) && x.Value.Equals(y.Value);
        }

        public int GetHashCode(SelectListItem obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            var hashText = obj.Text?.GetHashCode() ?? 0;
            var hashValue = obj.Value.GetHashCode();

            return hashText ^ hashValue;
        }
    }
}