using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminEventDetailsPage : Page
    {
        public AdminEventDetailsPage(IWebDriver driver) : base(driver)
        {
        }

        public IWebElement CreateNewTask =>
            _driver.FindElement(By.XPath($"//a[contains(@href, '{@"/Admin/VolunteerTask/Create/"}')]"));

        public AdminVolunteerTaskCreatePage ClickCreateNewTask()
        {
            CreateNewTask.Click();
            return new AdminVolunteerTaskCreatePage(_driver);
        }
    }
}
