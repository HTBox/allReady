using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminSitePage : Page
    {
        public AdminSitePage(IWebDriver driver) : base(driver)
        {
            Title = "Site Admin - allReady";
        }
    }
}
