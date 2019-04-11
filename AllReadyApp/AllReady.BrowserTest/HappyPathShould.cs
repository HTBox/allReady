using AllReady.BrowserTest.Models;
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

            loginPage.UserEmail.SendKeys(user.Name);
            loginPage.UserPassword.SendKeys(user.Password);

            var page = loginPage.Submit();
            Assert.StartsWith(roleReturnedPageTitle, _driver.Title);

            var homePage = page.Menu.Logoff();
            Assert.Equal(_driver.Title, homePage.Title);
        }

        static string UniqueName(string s)
        {
            string tag = DateTime.Now.ToString("yyyyMMddHHmmss");
            string name = $"[ST] {s} {tag}";

            return name;
        }

        public static IEnumerable<object[]> TestDataForCreateNewOrganization()
        {
            yield return new object[]
            {
                User.Role.AllReadyAdministrator,
                new Organization
                {
                    Name = UniqueName("Organization"),
                    PrivacyPolicy = "Privacy for all",
                    LocationAddress1 = "1 Microsoft Way",
                    LocationCity = "Redmond",
                    LocationState = "WA",
                    LocationPostalCode = "98052",
                    LocationCountry = "US"
                }
            };
        }

        Page Login(IWebDriver driver, User.Role role)
        {
            var user = new User(role);
            var loginPage = new Page(_driver).Menu.OpenLoginPage();

            var page = loginPage.LoginAs(user.Name, user.Password);
            Assert.Null(page.Title);

            return page;
        }

        [Theory]
        [MemberData(nameof(TestDataForCreateNewOrganization))]
        public void CreateNewOrganization(User.Role role, Organization organization)
        {
            var page = Login(_driver, role).Menu.OpenAdminOrganizationPage();
            Assert.Equal(_driver.Title, page.Title);

            var createPage = page.ClickCreateNew();
            Assert.Equal(_driver.Title, createPage.Title);

            // fill in form
            createPage.Name.SendKeys(organization.Name);
            // need to click privacy policy text button for PrivacyPolicy to appear
            createPage.PrivacyPolicyTextButton.Click();
            createPage.PrivacyPolicy.SendKeys(organization.PrivacyPolicy);
            // location information
            createPage.LocationAddress1.SendKeys(organization.LocationAddress1);
            createPage.LocationCity.SendKeys(organization.LocationCity);
            createPage.LocationState.SendKeys(organization.LocationState);
            createPage.LocationPostalCode.SendKeys(organization.LocationPostalCode);
            createPage.LocationCountry.SendKeys(organization.LocationCountry);

            var detailsPage = createPage.Submit();
            Assert.Equal($"{organization.Name} - allReady", _driver.Title);

            var homePage = detailsPage.Menu.Logoff();
            Assert.Equal(_driver.Title, homePage.Title);
        }

        public static IEnumerable<object[]> TestDataForCreateNewCampaign()
        {
            yield return new object[]
            {
                User.Role.AllReadyAdministrator,
                new Campaign
                {
                    Name = UniqueName("Campaign by Admin"),
                    Description = "Description",
                    Headline = "Headline",
                    FullDesciption = "Longer description of campaign",
                    TimeZone = "(UTC-08:00) Pacific Time (US & Canada)",
                    StartDate = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy"),
                    Organization = null,
                    Published = true
                }
            };
            yield return new object[]
            {
                User.Role.OrganizationAdministrator,
                new Campaign
                {
                    Name = UniqueName("Campaign by Org Admin"),
                    Description = "Description",
                    Headline = "Headline",
                    FullDesciption = "Longer description of campaign",
                    TimeZone = "(UTC-08:00) Pacific Time (US & Canada)",
                    StartDate = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy"),
                    Organization = null,
                    Published = true
                }
            };
        }

        [Theory]
        [MemberData(nameof(TestDataForCreateNewCampaign))]
        public void CreateNewCampaign(User.Role role, Campaign campaign)
        {
            var page = Login(_driver, role).Menu.OpenAdminCampaignPage();
            Assert.Equal(_driver.Title, page.Title);

            var createPage = page.ClickCreateNew();
            Assert.Equal(_driver.Title, createPage.Title);

            // need an organization to complete form
            campaign.Organization = campaign.Organization ?? createPage.Organization.GetLastItem();

            // fill in form
            createPage.Name.SendKeys(campaign.Name);
            createPage.Description.SendKeys(campaign.Description);
            createPage.Headline.SendKeys(campaign.Headline);
            createPage.FullDesciption.SendKeys(campaign.FullDesciption);
            createPage.TimeZone.SelectByText(campaign.TimeZone);
            createPage.StartDate.SendKeys(campaign.StartDate);
            createPage.Organization.SelectByText(campaign.Organization);
            createPage.Published.Checked(true);
            // copy contact information and confirm
            createPage.CopyContactInfoButton.Click();
            createPage.CopyConfirmDialog.ClickOK();
            //
            var detailsPage = createPage.Submit();
            Assert.Equal($"{campaign.Name} - allReady", _driver.Title);

            var homePage = detailsPage.Menu.Logoff();
            Assert.Equal(_driver.Title, homePage.Title);
        }
    }
}
