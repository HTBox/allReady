using AllReady.BrowserTest.Pages;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AllReady.BrowserTest
{
    /// <summary>
    /// Happy path for AllReady Admin
    /// <para>
    /// This set of tests are stateful and run in priority order.
    /// Preconditions of individual test are dependent on the previous
    /// test being successful.
    /// </para>
    /// </summary>
    [TestCaseOrderer("AllReady.BrowserTest.PriorityOrderer", "AllReady.BrowserTest")]
    public class AllReadyAdminHappyPathShould : IClassFixture<BrowserFixture>
    {
        IWebDriver _driver;
        IConfiguration _config;

        public AllReadyAdminHappyPathShould(BrowserFixture fixture)
        {
            _driver = fixture._driver;
            _config = new ConfigurationFixture().Config;
        }

        [Fact, TestPriority(1)]
        public void OpenHomePage()
        {
            _driver.Navigate().GoToUrl(_config["HomePageURL"]);

            var expected = "Home Page - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(2)]
        public void OpenLoginPage()
        {
            var page = new Page(_driver);
            page.Menu.LoginMenuItem.Click();

            var expected = "Log in - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(3)]
        public void Login()
        {
            var loginPage = new AccountLoginPage(_driver);

            loginPage.UserEmail.SendKeys(_config["AllReadyAdministratorUserEmail"]);
            loginPage.UserPassword.SendKeys(_config["AllReadyAdministratorPassword"]);
            loginPage.LoginButton.Click();

            var expected = "Site Admin - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(4)]
        public void OpenCurrentlyActiveOrganizationsPage()
        {
            var page = new Page(_driver);
            page.Menu.AdminOrganizationsMenuItem.Click();

            var expected = "Currently active organizations - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(5)]
        public void OpenCreateOrganizationPage()
        {
            var adminOrganizationPage = new AdminOgranizationPage(_driver);
            adminOrganizationPage.CreateOrgranizationButton.Click();

            var expected = "Create Organization - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        string UniqueName(string s)
        {
            string tag = DateTime.Now.ToString("yyyyMMddHHmmss");
            string name = $"[ST] {s} {tag}";

            return name;
        }

        [Fact, TestPriority(6)]
        public void CreateOrganization()
        {
            string organizationName = UniqueName("Organization");

            var adminOrganizationCreatePage = new AdminOrganizationCreatePage(_driver);

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

            var expected = $"{organizationName} - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(7)]
        public void OpenCampaignsAdminPage()
        {
            var page = new Page(_driver);
            page.Menu.AdminCampaignsMenuItem.Click();

            var expected = "Campaigns - Admin - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(8)]
        public void OpenCreateCampaignPage()
        {
            var adminCampaignPage = new AdminCampaignPage(_driver);
            adminCampaignPage.CreateCampaignButton.Click();

            var expected = "Create Campaign - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(9)]
        public void CreateCampaign()
        {
            string campaignName = UniqueName("Campaign");

            var adminCampaignCreatePage = new AdminCampaignCreatePage(_driver);

            adminCampaignCreatePage.Name.SendKeys(campaignName);
            adminCampaignCreatePage.Description.SendKeys("Description");
            adminCampaignCreatePage.Headline.SendKeys("Headline");
            adminCampaignCreatePage.FullDesciption.SendKeys("Longer desription of the campaign");
            adminCampaignCreatePage.TimeZone.SelectByText("Pacific", true);
            adminCampaignCreatePage.StartDate.SendKeys("04/15/2019");
            adminCampaignCreatePage.Organization.SelectLast();
            adminCampaignCreatePage.Published.Checked(true);
            adminCampaignCreatePage.CopyContactInfoButton.Click();
            adminCampaignCreatePage.CopyConfirmDialog.ClickOK();

            adminCampaignCreatePage.Submit();

            var expected = $"{campaignName} - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(10)]
        public void OpenCreateEventPage()
        {
            // pre-condition - /Admin/Campaign/Details/
            Assert.Contains(@"/Admin/Campaign/Details/", _driver.Url);

            // do the work
            var adminCampaignDetailsPage = new AdminCampaignDetailsPage(_driver);
            adminCampaignDetailsPage.CreateNewEvent.Click();

            // check that we got there
            const string partialDestinationUrl = @"/Admin/Event/Create/";
            Assert.Contains(partialDestinationUrl, _driver.Url);
        }

        [Fact, TestPriority(11)]
        public void CreateEvent()
        {
            string eventName = UniqueName("Event");

            var adminEventCreatePage = new AdminEventCreatePage(_driver);
            adminEventCreatePage.Name.SendKeys(eventName);
            adminEventCreatePage.Description.SendKeys("Description");
            adminEventCreatePage.Headline.SendKeys("Headline");
            adminEventCreatePage.EventType.SelectByText("Itinerary", true);
            adminEventCreatePage.IsLimitVolunteers.Checked(true);
            adminEventCreatePage.TimeZone.SelectByText("Pacific", true);
            adminEventCreatePage.StartDateTime.SendKeys("04/15/2019 9:00 AM");
            adminEventCreatePage.CopyLocationFromCampaignButton.Click();
            adminEventCreatePage.CopyConfirmDialog.ClickOK();
            adminEventCreatePage.Submit();

            var expected = $"{eventName} - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(12)]
        public void OpenCreateVolunteerTaskPage()
        {
            // pre-condition
            Assert.Contains(@"/Admin/Event/Details/", _driver.Url);

            // do work
            var adminEventDetailsPage = new AdminEventDetailsPage(_driver);
            adminEventDetailsPage.CreateNewTask.Click();

            // post-condition
            const string partialDestinationUrl = @"/Admin/VolunteerTask/Create/";
            Assert.Contains(partialDestinationUrl, _driver.Url);
        }

        [Fact, TestPriority(13)]
        public void CreateVolunteerTask()
        {
            string volunteerTaskName = UniqueName("Task");

            var adminVolunteerTaskCreatePage = new AdminVolunteerTaskCreatePage(_driver);

            adminVolunteerTaskCreatePage.Name.SendKeys(volunteerTaskName);
            adminVolunteerTaskCreatePage.Description.SendKeys("Description");
            adminVolunteerTaskCreatePage.NumberOfVolunteersRequired.SendKeys("2");
            adminVolunteerTaskCreatePage.StartDateTime.SendKeys("04/15/2019 9:00 AM");

            var eventName = _driver.FindElement(By.XPath(@"//div[contains(@class,'form-group')]//label[contains(@for,'EventName')]/following-sibling::div[1]")).Text;

            /// submit form
            adminVolunteerTaskCreatePage.Submit();

            var expected = $"{eventName} - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(99)]
        public void Logoff()
        {
            var homePage = new HomePage(_driver);
            homePage.Menu.LogoffMenuItem.Click();

            var expected = "Home Page - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }
    }
}
