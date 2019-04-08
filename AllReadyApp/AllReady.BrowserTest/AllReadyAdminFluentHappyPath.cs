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
                .Set(p => p.UserEmail, _config["AllReadyAdministratorUserEmail"])
                .Set(p => p.UserPassword, _config["AllReadyAdministratorPassword"])
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
            Assert.Equal("Site Admin - allReady", _driver.Title);

            var adminOrganizationPage = adminSitePage.Menu.OpenAdminOrganizationPage();
            Assert.Equal("Currently active organizations - allReady", _driver.Title);

            var adminOrganizationCreatePage = adminOrganizationPage.ClickCreateNew();
            Assert.Equal("Create Organization - allReady", _driver.Title);

            string organizationName = UniqueName("Organization");

            var adminOrganizationDetatilsPage = adminOrganizationCreatePage
                .Set(p => p.Name, organizationName)
                // need to click privacy policy text button for PrivacyPolicy to appear
                .Click(p => p.PrivacyPolicyTextButton)
                .SetControl(p => p.PrivacyPolicy, "Privacy for all")
                // location information
                .Set(p => p.LocationAddress1, "123 Main St")
                .Set(p => p.LocationCity, "Hollywood")
                .Set(p => p.LocationState, "CA")
                .Set(p => p.LocationPostalCode, "99120")
                .Set(p => p.LocationCountry, "USA")
                // 
                .Submit();

            Assert.Equal($"{organizationName} - allReady", _driver.Title);

            var homePage = adminOrganizationDetatilsPage.Menu.Logoff();
            Assert.Equal("Home Page - allReady", _driver.Title);
        }
    }
}