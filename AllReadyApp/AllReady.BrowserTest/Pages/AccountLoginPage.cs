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
            Title = "Log in - allReady";
        }

        public IWebElement UserEmail => _driver.FindElement(By.Id("Email"));
        public IWebElement UserPassword => _driver.FindElement(By.Id("Password"));
        public IWebElement LoginButton => _driver.FindElement(By.Id("login-submit"));

        /// <summary>
        /// Clicks login button on page
        /// </summary>
        /// <returns>Page based on the role of the logged in user</returns>
        public Page Submit()
        {
            LoginButton.Click();
            return new Page(_driver);
        }

        public Page LoginAs(string userEmail, string userPassword)
        {
            UserEmail.SendKeys(userEmail);
            UserPassword.SendKeys(userPassword);
            LoginButton.Click();
            return new Page(_driver);
        }
    }
}
