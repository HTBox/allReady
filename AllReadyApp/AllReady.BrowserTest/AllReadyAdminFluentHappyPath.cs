using AllReady.BrowserTest.Pages;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AllReady.BrowserTest
{
    public class AllReadyAdminFluentHappyPath
    {
        [Fact]
        public void ShouldLogonAndLogoff()
        {
            using (IWebDriver _driver = new BrowserFixture()._driver)
            {
                IConfiguration _config = new ConfigurationFixture().Config;
                _driver.Navigate().GoToUrl(_config["HomePageURL"]);

                var homePage = new Page(_driver).Menu.OpenHomePage();
                Assert.Equal("Home Page - allReady", _driver.Title);

                var loginPage = homePage.Menu.OpenLoginPage();
                Assert.Equal("Log in - allReady", _driver.Title);

                var adminSitePage = loginPage
                    .SetUserEmail(_config["AllReadyAdministratorUserEmail"])
                    .SetUserPassword(_config["AllReadyAdministratorPassword"])
                    .Submit();
                Assert.Equal("Site Admin - allReady", _driver.Title);

                adminSitePage.Menu.Logoff();
                Assert.Equal("Home Page - allReady", _driver.Title);
            }
        }

        [Fact]
        public void ShouldCreateNewOrganization()
        {

        }
    }
}
