using AllReady.BrowserTest.Controls;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminOrganizationCreatePage : Page
    {
        public AdminOrganizationCreatePage(IWebDriver driver) : base(driver)
        {
        }

        public IWebElement Name => _driver.FindElement(By.Id("Name"));
        public IWebElement PrivacyPolicyTextButton => _driver.FindElement(By.Id("show-pp-text"));
        public TinyMCE PrivacyPolicy => new TinyMCE(_driver, "PrivacyPolicy_ifr");
        public IWebElement LocationAddress1 => _driver.FindElement(By.Id("Location_Address1"));
        public IWebElement LocationCity => _driver.FindElement(By.Id("Location_City"));
        public IWebElement LocationState => _driver.FindElement(By.Id("Location_State"));
        public IWebElement LocationPostalCode => _driver.FindElement(By.Id("Location_PostalCode"));
        public IWebElement LocationCountry => _driver.FindElement(By.Id("Location_Country"));

        public AdminOrganizationDetailsPage Submit()
        {
            Name.Submit(); // use arbitrary element to submit form
            return new AdminOrganizationDetailsPage(_driver);
        }
    }
}
