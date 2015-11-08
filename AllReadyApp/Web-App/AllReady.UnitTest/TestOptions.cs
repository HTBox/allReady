
using System.Linq;
using Microsoft.Framework.OptionsModel;

namespace AllReady.UnitTest
{
    public class TestOptions<T> : OptionsManager<T> where T : class, new()
    {
        public TestOptions()
            : base(Enumerable.Empty<IConfigureOptions<T>>())
        {
        }
    }
}
