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
            _uiCulture = CultureInfo.CurrentUICulture;
            _culture = CultureInfo.CurrentCulture;

            CultureInfo.CurrentUICulture = contextualCulture;
            CultureInfo.CurrentCulture = contextualCulture;
        }

        public void Dispose()
        {
            CultureInfo.CurrentUICulture = _uiCulture;
            CultureInfo.CurrentCulture = _culture;
        }
    }
}
