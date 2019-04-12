using AllReady.BrowserTest.Controls;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminVolunteerTaskCreatePage : Page
    {
        public AdminVolunteerTaskCreatePage(IWebDriver driver) : base(driver)
        {
            Title = "Create Task - allReady";
        }

        public IWebElement Name => _driver.FindElement(By.Id("Name"));
        public IWebElement Description => _driver.FindElement(By.Id("Description"));
        public Spinner NumberOfVolunteersRequired => new Spinner(_driver, "NumberOfVolunteersRequired");
        public DateTimePicker StartDateTime => new DateTimePicker(_driver, "StartDateTime");

        public AdminEventDetailsPage Submit()
        {
            Name.Submit();
            return new AdminEventDetailsPage(_driver);
        }
    }
}
