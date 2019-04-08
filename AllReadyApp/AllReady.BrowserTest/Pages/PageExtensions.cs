using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    static class PageExtensions
    {
        public static T Set<T, TProperty>(this T t, Func<T, TProperty> p, string s) where TProperty : IWebElement
        {
            p.Invoke(t).SendKeys(s);

            return t;
        }

        public static T SetControl<T, TProperty>(this T t, Func<T, TProperty> p, string s) where TProperty : IControl
        {
            p.Invoke(t).Set(s);

            return t;
        }

        public static T Click<T, TProperty>(this T t, Func<T, TProperty> p) where TProperty : IWebElement
        {
            p.Invoke(t).Click();
            return t;
        }
    }
}
