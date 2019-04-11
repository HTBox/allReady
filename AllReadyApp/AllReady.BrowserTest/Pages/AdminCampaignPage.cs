using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminCampaignPage : Page
    {
        public AdminCampaignPage(IWebDriver driver) : base(driver)
        {
            Title = "Campaigns - Admin - allReady";
        }

        public IWebElement CreateCampaignButton => _driver.FindElement(By.LinkText("Create Campaign"));

        public AdminCampaignCreatePage ClickCreateNew()
        {
            CreateCampaignButton.Click();
            return new AdminCampaignCreatePage(_driver);
        }
    }
}
