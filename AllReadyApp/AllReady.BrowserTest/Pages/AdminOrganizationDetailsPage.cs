using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest
{
    class AdminOrganizationDetailsPage
    {
        IWebDriver _driver;

        public AdminOrganizationDetailsPage(IWebDriver driver)
        {
            _driver = driver;
        }
    }
}
