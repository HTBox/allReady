using System;
using System.Globalization;

namespace AllReady.UnitTest.Extensions
{
    public class CultureContext : IDisposable
    {
        private readonly CultureInfo _uiCulture = null;
        private readonly CultureInfo _culture = null;

        public CultureContext(CultureInfo contextualCulture)
        {
            _uiCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            _culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = contextualCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture= contextualCulture;
        }

        public void Dispose()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = _uiCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = _culture;
        }
    }
}
