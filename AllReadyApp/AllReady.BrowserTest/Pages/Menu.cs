using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class Menu
    {
        IWebDriver _driver;

        public Menu(IWebDriver driver)
        {
            _driver = driver;
        }

        public IWebElement LoginMenuItem => _driver.FindElement(By.ClassName("log-in"));

        IWebElement AdminMenuDropDown(string item)
        {
            // hover over dropdown element until dropdown appears
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1));
            var element = wait.Until<IWebElement>((_driver) =>
            {
                var e = _driver.FindElement(By.ClassName("dropdown-admin"));
                return e.Displayed ? e : null;
            });
            Actions action = new Actions(_driver);
            action.MoveToElement(element).Perform();

            // TODO - change to CSS selector possibly
            //_driver.FindElement(By.XPath(@"//a[(@href='/Admin/Organization')]")).Click();
            //element = _driver.FindElement(By.XPath(@"//li[contains(@class,'dropdown-admin')]//a[text()='Organizations']"));
            var xpath = $"//li[contains(@class,'dropdown-admin')]//a[text()='{item}']";
            element = _driver.FindElement(By.XPath(xpath));

            return element;
        }

        IWebElement AccountMenuDropDown(string item)
        {
            // hover over dropdown element until dropdown appears
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1));
            var element = wait.Until<IWebElement>((_driver) =>
            {
                var e = _driver.FindElement(By.ClassName("dropdown-account"));
                return e.Displayed ? e : null;
            });
            Actions action = new Actions(_driver);
            action.MoveToElement(element).Perform();

            // click log out button
            element = _driver.FindElement(By.ClassName(item));

            return element;
        }

        public IWebElement AdminOrganizationMenuItem => AdminMenuDropDown("Organizations");
        public IWebElement LogoffMenuItem => AccountMenuDropDown("log-out");

        /// <summary>
        /// Opens login page.
        /// <para>Only available if user is not logged in.</para>
        /// </summary>
        /// <returns>Login page</returns>
        public AccountLoginPage OpenLoginPage()
        {
            LoginMenuItem.Click();
            return new AccountLoginPage(_driver);
        }

        /// <summary>
        /// Logs current user off and returns home page.
        /// <para>Note: Only available if user is logged in.</para>
        /// </summary>
        /// <returns>Home page</returns>
        public HomePage Logoff()
        {
            LogoffMenuItem.Click();
            return new HomePage(_driver);
        }

        public AdminOgranizationPage OpenAdminOrganizationPage()
        {
            return new AdminOgranizationPage(_driver);
        }
    }
}
