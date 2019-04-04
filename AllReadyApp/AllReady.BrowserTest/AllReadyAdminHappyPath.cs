using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AllReady.BrowserTest
{
    [TestCaseOrderer("AllReady.BrowserTest.PriorityOrderer", "AllReady.BrowserTest")]
    public class AllReadyAdminHappyPath : IClassFixture<BrowserFixture>
    {
        BrowserFixture _fixture;
        IWebDriver _driver;
        IConfiguration _config;

        public AllReadyAdminHappyPath(BrowserFixture fixture)
        {
            _fixture = fixture;
            _driver = fixture._driver;
            _config = fixture._config;
        }

        [Fact, TestPriority(1)]
        public void ShouldOpenHomePage()
        {
            _driver.Navigate().GoToUrl(_config["HomePageURL"]);

            var expected = "Home Page - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(2)]
        public void ShouldOpenLoginPage()
        {
            _driver.FindElement(By.ClassName("log-in")).Click();
            var expected = "Log in - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(3)]
        public void ShouldLogin()
        {
            _driver.FindElement(By.Id("Email")).SendKeys(_config["AllReadyAdministratorUserEmail"]);
            _driver.FindElement(By.Id("Password")).SendKeys(_config["AllReadyAdministratorPassword"]);
            _driver.FindElement(By.Id("login-submit")).Click();

            var expected = "Site Admin - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(4)]
        public void ShouldOpenCurrentlyActiveOrganizationsPage()
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

            //
            //_driver.FindElement(By.XPath(@"//a[(@href='/Admin/Organization')]")).Click();
            _driver.FindElement(By.XPath(@"//li[contains(@class,'dropdown-admin')]//a[text()='Organizations']")).Click();

            var expected = "Currently active organizations - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(5)]
        public void ShouldOpenCreateOrganizationPage()
        {
            _driver.FindElement(By.LinkText("Create Organization")).Click();

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
        public void ShouldCreateOrganization()
        {
            string organizationName = UniqueName("Organization");

            /// filling in only required fields
            // enter orgainization information
            _driver.FindElement(By.Id("Name")).SendKeys(organizationName);

            /// enter privacy policy text
            // 
            _driver.FindElement(By.Id("show-pp-text")).Click();
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1));
            var element = wait.Until<IWebElement>((_driver) =>
            {
                var e = _driver.FindElement(By.Id("PrivacyPolicy_ifr"));
                return e.Displayed ? e : null;
            });
            Actions action = new Actions(_driver);
            action.MoveToElement(element).Perform();
            // 
            _driver.SwitchTo().Frame(_driver.FindElement(By.Id("PrivacyPolicy_ifr")));
            _driver.FindElement(By.Id("tinymce")).SendKeys("Privacy for all");
            _driver.SwitchTo().DefaultContent();

            /// enter primary location information
            _driver.FindElement(By.Id("Location_Address1")).SendKeys("123 Main St");
            _driver.FindElement(By.Id("Location_City")).SendKeys("Hollywood");
            _driver.FindElement(By.Id("Location_State")).SendKeys("CA");
            _driver.FindElement(By.Id("Location_PostalCode")).SendKeys("99120");
            _driver.FindElement(By.Id("Location_Country")).SendKeys("USA");

            /// enter primary contact information
            //

            /// submit form
            element.Submit();

            var expected = $"{organizationName} - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(7)]
        public void ShouldOpenCampaignsAdminPage()
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

            //
            _driver.FindElement(By.XPath(@"//li[contains(@class,'dropdown-admin')]//a[text()='Campaigns']")).Click();

            var expected = "Campaigns - Admin - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(8)]
        public void ShouldOpenCreateCampaignPage()
        {
            _driver.FindElement(By.LinkText("Create Campaign")).Click();

            var expected = "Create Campaign - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(9)]
        public void ShouldCreateCampaign()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1));
            string campaignName = UniqueName("Campaign");

            _driver.FindElement(By.Id("Name")).SendKeys(campaignName);
            _driver.FindElement(By.Id("Description")).SendKeys("description");
            _driver.FindElement(By.Id("Headline")).SendKeys("headline");

            _driver.SwitchTo().Frame(_driver.FindElement(By.Id("FullDescription_ifr")));
            _driver.FindElement(By.Id("tinymce")).SendKeys("Longer desription of the campaign");
            _driver.SwitchTo().DefaultContent();

            // set time zone from fixed dropdown list
            new SelectElement(_driver.FindElement(By.Id("TimeZoneId")))
                .SelectByText("Pacific", true);

            // set start date
            var d = _driver.FindElement(By.Id("StartDate"));
            d.Click();
            wait.Until<IWebElement>(_driver =>
            {
                var e = _driver.FindElement(By.ClassName("bootstrap-datetimepicker-widget"));
                return e.Displayed ? e : null;
            });
            d = _driver.FindElement(By.Id("StartDate"));
            d.SendKeys(Keys.Control + "a");
            d.SendKeys("04/15/2019");

            // select organization
            var organizationList = new SelectElement(_driver.FindElement(By.Id("OrganizationId")));
            organizationList.SelectByIndex(organizationList.Options.Count - 1);
            //

            _driver.FindElement(By.Id("Published")).Click();
            // copy location from primary contact
            _driver.FindElement(By.Id("btnGetContactInfo")).Click();
            var confirmCopyContactModal = wait.Until<IWebElement>((_driver) =>
            {
                var e = _driver.FindElement(By.Id("confirmContactModal"));
                return e.Displayed ? e : null;
            });
            Actions action = new Actions(_driver);
            action.MoveToElement(confirmCopyContactModal).Perform();
            //
            _driver.FindElement(By.Id("confirmOverwriteContact")).Click();
            //
            wait.Until<IWebElement>((_driver) =>
            {
                return confirmCopyContactModal.Displayed ? null : confirmCopyContactModal;
            });

            /// submit form
            _driver.FindElement(By.Id("campaign-form")).Submit();

            var expected = $"{campaignName} - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(10)]
        public void ShouldOpenCreateEventPage()
        {
            const string partialDestinationUrl = @"/Admin/Event/Create/";
            // pre-condition - /Admin/Campaign/Details/
            Assert.Contains(@"/Admin/Campaign/Details/", _driver.Url);
            // find - href Admin/Event/Create/
            _driver.FindElement(By.XPath($"//a[contains(@href, '{partialDestinationUrl}')]")).Click();
            Assert.Contains(partialDestinationUrl, _driver.Url);
        }

        [Fact, TestPriority(11)]
        public void ShouldCreateEvent()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1));
            string eventName = UniqueName("Event");

            _driver.FindElement(By.Id("Name")).SendKeys(eventName);
            _driver.FindElement(By.Id("Description")).SendKeys("description");
            _driver.FindElement(By.Id("Headline")).SendKeys("headline");
            new SelectElement(_driver.FindElement(By.Id("EventType")))
                .SelectByText("Itinerary", true);
            //var element = _driver.FindElement(By.Id("IsLimitVolunteers"));
            //if (!element.Selected)
            //{
            //    element.Click();
            //}

            new SelectElement(_driver.FindElement(By.Id("TimeZoneId")))
                .SelectByText("Pacific", true);

            var element = _driver.FindElement(By.Id("StartDateTime"));
            element.SendKeys(Keys.Control + "a");
            wait.Until(_driver => _driver.FindElements(By.ClassName("bootstrap-datetimepicker-widget")).Count > 0);
            element.SendKeys("04/15/2019 9:00 AM");
            element.SendKeys(Keys.Escape);
            wait.Until(_driver => _driver.FindElements(By.ClassName("bootstrap-datetimepicker-widget")).Count == 0);

            // copy location from campaign
            _driver.FindElement(By.Id("btnGetLocationInfo")).Click();
            var confirmCopyContactModal = wait.Until<IWebElement>((_driver) =>
            {
                var e = _driver.FindElement(By.Id("confirmLocationModal"));
                return e.Displayed ? e : null;
            });
            _driver.FindElement(By.Id("confirmOverwriteLocation")).Click();
            wait.Until(_driver => !_driver.FindElement(By.Id("confirmLocationModal")).Displayed);

            /// submit form
            _driver.FindElement(By.ClassName("submit-form")).Submit();

            var expected = $"{eventName} - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(12)]
        public void ShouldOpenCreateVolunteerTaskPage()
        {
            const string partialDestinationUrl = @"/Admin/VolunteerTask/Create/";
            Assert.Contains(@"/Admin/Event/Details/", _driver.Url);
            _driver.FindElement(By.XPath($"//a[contains(@href, '{partialDestinationUrl}')]")).Click();
            Assert.Contains(partialDestinationUrl, _driver.Url);
        }

        [Fact, TestPriority(13)]
        public void ShouldCreateVolunteerTask()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1));
            string volunteerTaskName = UniqueName("Task");

            _driver.FindElement(By.Id("Name")).SendKeys(volunteerTaskName);
            _driver.FindElement(By.Id("Description")).SendKeys("description");
            _driver.FindElement(By.Id("NumberOfVolunteersRequired")).SendKeys("2");

            var element = _driver.FindElement(By.Id("StartDateTime"));
            element.SendKeys(Keys.Control + "a");
            wait.Until(_driver => _driver.FindElements(By.ClassName("bootstrap-datetimepicker-widget")).Count > 0);
            element.SendKeys("04/15/2019 9:00 AM");
            element.SendKeys(Keys.Escape);
            wait.Until(_driver => _driver.FindElements(By.ClassName("bootstrap-datetimepicker-widget")).Count == 0);

            var eventName = _driver.FindElement(By.XPath(@"//div[contains(@class,'form-group')]//label[contains(@for,'EventName')]/following-sibling::div[1]")).Text;

            /// submit form
            element.Submit();

            var expected = $"{eventName} - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }

        [Fact, TestPriority(99)]
        public void ShouldLogoff()
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
            _driver.FindElement(By.ClassName("log-out")).Click();

            var expected = "Home Page - allReady";
            var actual = _driver.Title;
            Assert.Equal(expected, actual);
        }
    }
}
