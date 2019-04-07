using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminOgranizationPage : Page
    {
        public AdminOgranizationPage(IWebDriver driver) : base(driver)
        {
        }

        /// <summary>
        /// Click opens AdminOrganizationCreate page in browser
        /// </summary>
        public IWebElement CreateOrgranizationButton => _driver.FindElement(By.LinkText("Create Organization"));
    }
}
