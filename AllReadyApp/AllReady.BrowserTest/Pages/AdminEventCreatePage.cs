using AllReady.BrowserTest.Controls;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminEventCreatePage : Page
    {
        public AdminEventCreatePage(IWebDriver driver) : base(driver)
        {
            Title = "Create Event - allReady";
        }

        public IWebElement Name => _driver.FindElement(By.Id("Name"));
        public IWebElement Description => _driver.FindElement(By.Id("Description"));
        public IWebElement Headline => _driver.FindElement(By.Id("Headline"));
        public DropDown EventType => new DropDown(_driver, "EventType");
        public CheckBox IsLimitVolunteers => new CheckBox(_driver, "IsLimitVolunteers");
        public DropDown TimeZone => new DropDown(_driver, "TimeZoneId");
        public DateTimePicker StartDateTime => new DateTimePicker(_driver, "StartDateTime");
        public IWebElement CopyLocationFromCampaignButton => _driver.FindElement(By.Id("btnGetLocationInfo"));
        public AdminEventCreateCopyConfirmDialog CopyConfirmDialog => new AdminEventCreateCopyConfirmDialog(_driver);

        public AdminEventDetailsPage Submit()
        {
            _driver.FindElement(By.ClassName("submit-form")).Submit();
            return new AdminEventDetailsPage(_driver);
        }
    }
}
