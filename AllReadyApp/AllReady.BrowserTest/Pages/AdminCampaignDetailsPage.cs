using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminCampaignDetailsPage
    {
        IWebDriver _driver;

        public AdminCampaignDetailsPage(IWebDriver driver)
        {
            _driver = driver;
        }

        const string partialDestinationUrl = @"/Admin/Event/Create/";
        public IWebElement CreateNewEvent => _driver.FindElement(By.XPath($"//a[contains(@href, '{partialDestinationUrl}')]"));
    }
}
