using AllReady.BrowserTest.Pages;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AllReady.BrowserTest
{
    public class AllReadyAdminFluentHappyPath : IClassFixture<BrowserFixture>
    {
        IWebDriver _driver;
        IConfiguration _config;

        public AllReadyAdminFluentHappyPath(BrowserFixture browserFixture)
        {
            _driver = browserFixture._driver;
            _config = new ConfigurationFixture().Config;

            _driver.Navigate().GoToUrl(_config["HomePageURL"]);
        }

        [Fact]
        public void ShouldOpenHomePage()
        {
            var homePage = new Page(_driver).Menu.OpenHomePage();
            Assert.Equal("Home Page - allReady", _driver.Title);
        }

        [Fact]
        public void ShouldLogonAndLogoff()
        {
            var loginPage = new Page(_driver).Menu.OpenLoginPage();
            Assert.Equal("Log in - allReady", _driver.Title);

            var adminSitePage = loginPage
                .SetUserEmail(_config["AllReadyAdministratorUserEmail"])
                .SetUserPassword(_config["AllReadyAdministratorPassword"])
                .Submit();
            Assert.Equal("Site Admin - allReady", _driver.Title);

            var homePage = adminSitePage.Menu.Logoff();
            Assert.Equal("Home Page - allReady", _driver.Title);
        }

        string UniqueName(string s)
        {
            string tag = DateTime.Now.ToString("yyyyMMddHHmmss");
            string name = $"[ST] {s} {tag}";

            return name;
        }

        [Fact]
        public void ShouldCreateNewOrganization()
        {
            var loginPage = new Page(_driver).Menu.OpenLoginPage();
            var adminSitePage = loginPage.LoginAs(_config["AllReadyAdministratorUserEmail"], _config["AllReadyAdministratorPassword"]);

            var adminOrganizationPage = adminSitePage.Menu.OpenAdminOrganizationPage();
            adminOrganizationPage.CreateOrgranizationButton.Click();
            Assert.Equal("Create Organization - allReady", _driver.Title);

            var adminOrganizationCreatePage = new AdminOrganizationCreatePage(_driver);

            string organizationName = UniqueName("Organization");
            adminOrganizationCreatePage.Name.SendKeys(organizationName);

            // choose to enter privacy policy text
            adminOrganizationCreatePage.PrivacyPolicyTextButton.Click();
            adminOrganizationCreatePage.PrivacyPolicy.SendKeys("Privacy for all");

            /// enter primary location information
            adminOrganizationCreatePage.LocationAddress1.SendKeys("123 Main St");
            adminOrganizationCreatePage.LocationCity.SendKeys("Hollywood");
            adminOrganizationCreatePage.LocationState.SendKeys("CA");
            adminOrganizationCreatePage.LocationPostalCode.SendKeys("99120");
            adminOrganizationCreatePage.LocationCountry.SendKeys("USA");

            /// enter primary contact information
            //

            /// submit form
            adminOrganizationCreatePage.Submit();

        }
    }
}
