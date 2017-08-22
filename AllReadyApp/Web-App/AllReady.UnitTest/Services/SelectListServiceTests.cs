using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.Services;
using Xunit;
using System.Security.Claims;

namespace AllReady.UnitTest.Services
{
    public class SelectListServiceTests : InMemoryContextTest
    {
        private static int _organizationId1 = 1;
        private static string _organizationName1 = "Organization Name 1";

        [Fact]
        public void GetOrganizationsForAdminUserReturnsAllOrganizations()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin)) }));

            var service = new SelectListService(Context);
            var organizations = service.GetOrganizations( principal ).ToList();

            Assert.Equal(2, organizations.Count);
            Assert.Equal(_organizationId1.ToString(), organizations.First().Value);
            Assert.Equal(_organizationName1, organizations.First().Text);
        }

        [Fact]
        public void GetOrganizationsForOrgAdminUserReturnsOnlyAuthorizedOrganization()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, _organizationId1.ToString())
            }));

            var service = new SelectListService(Context);
            var organizations = service.GetOrganizations(principal).ToList();

            Assert.Single(organizations);
            Assert.Equal(_organizationId1.ToString(), organizations.First().Value);
            Assert.Equal(_organizationName1, organizations.First().Text);
        }

        [Fact]
        public void GetOrganizationsForOrgAdminUserWithNoAssociatedOrgReturnsNoOrganizations()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin))
            }));

            var service = new SelectListService(Context);
            var organizations = service.GetOrganizations(principal).ToList();

            Assert.Empty(organizations);
        }

        [Fact]
        public void GetOrganizationsForSimpleUserReturnsNoOrganizations()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(AllReady.Security.ClaimTypes.UserType, "User")
            }));

            var service = new SelectListService(Context);
            var organizations = service.GetOrganizations(principal).ToList();

            Assert.Empty(organizations);
        }

        [Fact]
        public void GetOrganizationsForNoClaimsReturnsNoOrganizations()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[0]));

            var service = new SelectListService(Context);
            var organizations = service.GetOrganizations(principal).ToList();

            Assert.Empty(organizations);
        }

        [Fact]
        public void GetSkills()
        {
            var data = new List<Skill>
            {
                new Skill { Id = 1, Description = "description1", Name = "c" },
                new Skill { Id = 2, Description = "description2", Name = "b" },
                new Skill { Id = 3, Description = "description3", Name = "a" }
            };

            data.ForEach(s => Context.Add(s));
            Context.SaveChanges();

            var service = new SelectListService(Context);
            var skills = service.GetSkills().ToList();

            var expected = data.OrderBy(x => x.Name).ToList();
            var notExpected = data.OrderByDescending(x => x.Name).ToList();

            Assert.Equal(3, skills.Count);
            Assert.Equal(expected, skills, new SkillComparer());
            // this check makes sure the equality comparer is functioning
            Assert.NotEqual(notExpected, skills, new SkillComparer());
        }

        private class SkillComparer : IEqualityComparer<Skill>
        {
            public bool Equals(Skill x, Skill y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null | y == null)
                    return false;
                return x.Id == y.Id 
                    && x.Name.Equals(y.Name, StringComparison.Ordinal)
                    && x.Description.Equals(y.Description, StringComparison.Ordinal);
            }

            public int GetHashCode(Skill obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        protected override void LoadTestData()
        {
            Context.Add(new Organization { Id = _organizationId1, Name = _organizationName1 });
            Context.Add(new Organization { Id = 2, Name = "Organization Name 2" });
            Context.SaveChanges();
        }
    }
}
