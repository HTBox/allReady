using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminCampaignDetailsPage : Page
    {
        public AdminCampaignDetailsPage(IWebDriver driver) : base(driver)
        {
        }

        const string partialDestinationUrl = @"/Admin/Event/Create/";
        public IWebElement CreateNewEvent => _driver.FindElement(By.XPath($"//a[contains(@href, '{partialDestinationUrl}')]"));

        public AdminEventCreatePage ClickCreateNewEvent()
        {
            CreateNewEvent.Click();
            return new AdminEventCreatePage(_driver);
        }
    }
}
