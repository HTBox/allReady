using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    /// <summary>
    /// All web pages inherit from this base class.
    /// <para>Contains common menu bar for all pages.</para>
    /// </summary>
    class Page
    {
        protected IWebDriver _driver;
        public Menu Menu;

        public Page(IWebDriver driver)
        {
            _driver = driver;
            Menu = new Menu(driver);
            Title = null;
        }

        public string Title { get; protected set; }
    }
}
