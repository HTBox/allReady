using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminCampaignPage : Page
    {
        public AdminCampaignPage(IWebDriver driver) : base(driver)
        {
            Title = "Campaigns - Admin - allReady";
            IReadOnlyCollection<IWebElement> el = _driver.FindElements(By.XPath("//table//tbody//tr"));
        }

        public IWebElement CreateCampaignButton => _driver.FindElement(By.LinkText("Create Campaign"));
        public ReadOnlyCollection<IWebElement> ListOfCampaigns => _driver.FindElements(By.XPath("//table//tbody//tr"));

        public AdminCampaignCreatePage ClickCreateNew()
        {
            CreateCampaignButton.Click();
            return new AdminCampaignCreatePage(_driver);
        }
    }
}
