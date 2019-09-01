using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminEventListAllPage : Page
    {
        public AdminEventListAllPage(IWebDriver driver) : base(driver)
        {
            Title = "Event Lister -allReady";
        }
    }
}
