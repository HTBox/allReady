using System;
using System.Collections.Generic;
using System.Security.Claims;
using AllReady.Controllers;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class DetermineIfAVolunteerTaskIsEditableShould: InMemoryContextTest
    {
        [Fact]
        public void SiteAdminsCanEditAllReadyTasks()
        {
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin))
            }));

            var sut = new DetermineIfATaskIsEditable();
            var result = sut.For(claimsPrincipal, null, UserManager);

            Assert.True(result);
        }

        [Fact]
        public void OrgAdminsCanEditAllReadyTasks()
        {
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin))
            }));

            var sut = new DetermineIfATaskIsEditable();
            var result = sut.For(claimsPrincipal, null, UserManager);

            Assert.True(result);
        }

        [Fact]
        public void VolunteerTaskThatHasInstanceOfEventAndEventHasInstanceOfOrganizerAndOrganizerIdEqualsUserIdIsEditable()
        {
            const string userId = "1";

            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.BasicUser))
            }));

            var volunteerTask = new VolunteerTask { Event = new Event { Organizer = new ApplicationUser { Id = userId }}};

            var sut = new DetermineIfATaskIsEditable();
            var result = sut.For(claimsPrincipal, volunteerTask, UserManager);

            Assert.True(result);
        }

        [Fact]
        public void VolunteerTaskThatHasInstanceOfEventAndEventHasInstanceOfCampaignAndCampaignHasInstanceOfOrganizerAndOrganizerIdEqualsUserIdIsEditable()
        {
            const string userId = "1";

            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.BasicUser))
            }));

            var volunteerTask = new VolunteerTask { Event = new Event { Campaign = new Campaign { Organizer = new ApplicationUser { Id = userId }}}};

            var sut = new DetermineIfATaskIsEditable();
            var result = sut.For(claimsPrincipal, volunteerTask, UserManager);

            Assert.True(result);
        }
    }
}
