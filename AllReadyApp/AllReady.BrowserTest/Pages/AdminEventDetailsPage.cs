using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminEventDetailsPage
    {
        IWebDriver _driver;

        public AdminEventDetailsPage(IWebDriver driver)
        {
            _driver = driver;
        }

        const string partialDestinationUrl = @"/Admin/VolunteerTask/Create/";
        public IWebElement CreateNewTask => _driver.FindElement(By.XPath($"//a[contains(@href, '{partialDestinationUrl}')]"));
    }
}
