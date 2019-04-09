using AllReady.BrowserTest.Pages;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AllReady.BrowserTest
{
    public class HappyPathShould : IClassFixture<BrowserFixture>
    {
        IWebDriver _driver;
        IConfiguration _config;

        public HappyPathShould(BrowserFixture browserFixture)
        {
            _driver = browserFixture._driver;
            _config = new ConfigurationFixture().Config;

            _driver.Navigate().GoToUrl(_config["HomePageURL"]);
        }

        [Fact]
        public void OpenHomePage()
        {
            var homePage = new Page(_driver).Menu.OpenHomePage();
            Assert.Equal(_driver.Title, homePage.Title);
        }

        /// <summary>
        /// Tests for successful logon and logoff
        /// <para>
        /// The login process will return different pages depending on the role
        /// the logged in user.
        /// </para>
        /// </summary>
        /// <param name="role">application role</param>
        /// <param name="roleReturnedPageTitle">partial title of page returned on successful login </param>
        [Theory]
        [InlineData(User.Role.AllReadyAdministrator, "Site Admin")]
        [InlineData(User.Role.OrganizationAdministrator, "Campaigns - Admin")]
        public void LogonAndLogoff(User.Role role, string roleReturnedPageTitle)
        {
            var user = new User(role);
            var loginPage = new Page(_driver).Menu.OpenLoginPage();
            Assert.Equal(_driver.Title, loginPage.Title);

            var page = loginPage
                .Set(p => p.UserEmail, user.Name)
                .Set(p => p.UserPassword, user.Password)
                .Submit();
            Assert.StartsWith(roleReturnedPageTitle, _driver.Title);

            var homePage = page.Menu.Logoff();
            Assert.Equal(_driver.Title, homePage.Title);
        }

        string UniqueName(string s)
        {
            string tag = DateTime.Now.ToString("yyyyMMddHHmmss");
            string name = $"[ST] {s} {tag}";

            return name;
        }

        [Theory]
        [InlineData(User.Role.AllReadyAdministrator)]
        public void CreateNewOrganization(User.Role role)
        {
            var user = new User(role);
            var loginPage = new Page(_driver).Menu.OpenLoginPage();

            var page = loginPage.LoginAs(user.Name, user.Password);
            Assert.Null(page.Title);

            var adminOrganizationPage = page.Menu.OpenAdminOrganizationPage();
            Assert.Equal(_driver.Title, adminOrganizationPage.Title);

            var adminOrganizationCreatePage = adminOrganizationPage.ClickCreateNew();
            Assert.Equal(_driver.Title, adminOrganizationCreatePage.Title);

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
            Assert.Equal(_driver.Title, homePage.Title);
        }
    }
}
