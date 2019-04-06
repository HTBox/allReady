using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminCampaignPage
    {
        IWebDriver _driver;

        public AdminCampaignPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public IWebElement CreateCampaignButton => _driver.FindElement(By.LinkText("Create Campaign"));
    }
}
