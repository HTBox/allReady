using System;
using System.Collections.Generic;
using System.Security.Claims;
using AllReady.Controllers;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class DetermineIfATaskIsEditableShould
    {
        [Fact(Skip = "RTM Broken Tests")]
        public void SiteAdminsCanEditAllReadyTasks()
        {
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof (UserType), UserType.SiteAdmin))
            }));

            var sut = new DetermineIfATaskIsEditable();
            var result = sut.For(claimsPrincipal, null, null);

            Assert.True(result);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public void OrgAdminsCanEditAllReadyTasks()
        {
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof (UserType), UserType.OrgAdmin))
            }));

            var sut = new DetermineIfATaskIsEditable();
            var result = sut.For(claimsPrincipal, null, null);

            Assert.True(result);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public void AllReadyTaskThatHasInstanceOfEventAndEventHasInstanceOfOrganizerAndOrganizerIdEqualsUserIdIsEditable()
        {
            const string userId = "1";

            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof (UserType), UserType.BasicUser))
            }));

            var allReadyTask = new AllReadyTask { Event = new Event { Organizer = new ApplicationUser { Id = userId }}};

            var sut = new DetermineIfATaskIsEditable();
            var result = sut.For(claimsPrincipal, allReadyTask, null);

            Assert.True(result);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public void AllReadyTaskThatHasInstanceOfEventAndEventHasInstanceOfCampaignAndCampaignHasInstanceOfOrganizerAndOrganizerIdEqualsUserIdIsEditable()
        {
            const string userId = "1";

            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof (UserType), UserType.BasicUser))
            }));

            var allReadyTask = new AllReadyTask { Event = new Event { Campaign = new Campaign { Organizer = new ApplicationUser { Id = userId }}}};

            var sut = new DetermineIfATaskIsEditable();
            var result = sut.For(claimsPrincipal, allReadyTask, null);

            Assert.True(result);
        }
    }
}
