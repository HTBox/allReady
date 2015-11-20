
using System.Linq;
using Microsoft.Extensions.OptionsModel;

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
