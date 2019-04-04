using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AllReady.BrowserTest
{
    public class BrowserFixture : IDisposable
    {
        public IWebDriver _driver;

        public BrowserFixture()
        {
            _driver = new ChromeDriver(Environment.CurrentDirectory);
            _driver.Manage().Window.Size = new System.Drawing.Size(1024, 768);
        }
        void IDisposable.Dispose()
        {
            _driver.Quit();
        }
    }
}
