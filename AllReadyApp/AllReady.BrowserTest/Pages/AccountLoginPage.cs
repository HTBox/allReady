using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AccountLoginPage : Page
    {
        public AccountLoginPage(IWebDriver driver) : base(driver)
        {
        }

        public IWebElement UserEmail => _driver.FindElement(By.Id("Email"));
        public IWebElement UserPassword => _driver.FindElement(By.Id("Password"));
        public IWebElement LoginButton => _driver.FindElement(By.Id("login-submit"));

        public AccountLoginPage SetUserEmail(string email)
        {
            UserEmail.SendKeys(email);
            return this;
        }

        public AccountLoginPage SetUserPassword(string password)
        {
            UserPassword.SendKeys(password);
            return this;
        }

        /// <summary>
        /// opens admin site page
        /// </summary>
        /// <returns>admin site page</returns>
        public AdminSitePage Submit()
        {
            LoginButton.Click();
            return new AdminSitePage(_driver);
        }

        public AdminSitePage LoginAs(string userEmail, string userPassword)
        {
            UserEmail.SendKeys(userEmail);
            UserPassword.SendKeys(userPassword);
            LoginButton.Click();
            return new AdminSitePage(_driver);
        }
    }
}
