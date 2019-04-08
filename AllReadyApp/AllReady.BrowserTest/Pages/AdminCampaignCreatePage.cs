using AllReady.BrowserTest.Controls;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminCampaignCreatePage
    {
        IWebDriver _driver;

        public AdminCampaignCreatePage(IWebDriver driver)
        {
            _driver = driver;
        }

        public IWebElement Name => _driver.FindElement(By.Id("Name"));
        public IWebElement Description => _driver.FindElement(By.Id("Description"));
        public IWebElement Headline => _driver.FindElement(By.Id("Headline"));
        public TinyMCE FullDesciption => new TinyMCE(_driver, "FullDescription_ifr");
        public DropDown TimeZone => new DropDown(_driver, "TimeZoneId");
        public DateTimePicker StartDate => new DateTimePicker(_driver, "StartDate");
        public DropDown Organization => new DropDown(_driver, "OrganizationId");
        public CheckBox Published => new CheckBox(_driver, "Published");
        public IWebElement CopyContactInfoButton => _driver.FindElement(By.Id("btnGetContactInfo"));
        public AdminCampaignCreateCopyConfirmDialog CopyConfirmDialog => new AdminCampaignCreateCopyConfirmDialog(_driver);

        public AdminCampaignDetailsPage Submit()
        {
            _driver.FindElement(By.Id("campaign-form")).Submit();
            return new AdminCampaignDetailsPage(_driver);
        }
    }
}
