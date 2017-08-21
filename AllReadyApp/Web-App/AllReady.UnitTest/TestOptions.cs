using System.Linq;
using Microsoft.Extensions.Options;

namespace AllReady.UnitTest
{
    public class TestOptions<T> : OptionsManager<T> where T : class, new()
    {
        public TestOptions()
            : base(new OptionsFactory<T>(Enumerable.Empty<IConfigureOptions<T>>(),
                Enumerable.Empty<IPostConfigureOptions<T>>()))
        {
        }
    }
}
