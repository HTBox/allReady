using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminCampaignDetailsPage : Page
    {
        public AdminCampaignDetailsPage(IWebDriver driver) : base(driver)
        {
        }

        public IWebElement CreateNewEvent =>
            _driver.FindElement(By.XPath($"//a[contains(@href, '{@"/Admin/Event/Create/"}')]"));
        public ReadOnlyCollection<IWebElement> ListOfEvents =>
            _driver.FindElements(By.XPath("(//div[contains(@class,'body-content')]//div[contains(@class,'row')])[8]//table//tr"));

        public AdminEventCreatePage ClickCreateNewEvent()
        {
            CreateNewEvent.Click();
            return new AdminEventCreatePage(_driver);
        }
    }
}
