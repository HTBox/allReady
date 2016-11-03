using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Security.Claims;

namespace AllReady.UnitTest.Services
{
    public class SelectListServiceTests
    {
        private static int _organizationId1 = 1;
        private static string _organizationName1 = "Organization Name 1";

        [Fact]
        public void GetOrganizationsForAdminUserReturnsAllOrganizations()
        {
            var mockSet = CreateOrganizationsInMockDataStore();

            var mockContext = new Mock<AllReadyContext>();
            mockContext.Setup(c => c.Organizations).Returns(mockSet.Object);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.UserType, "SiteAdmin") }));

            var service = new SelectListService(mockContext.Object);
            var organizations = service.GetOrganizations( principal ).ToList();

            Assert.Equal(2, organizations.Count);
            Assert.Equal(_organizationId1.ToString(), organizations.First().Value);
            Assert.Equal(_organizationName1, organizations.First().Text);
        }

        [Fact]
        public void GetOrganizationsForOrgAdminUserReturnsOnlyAuthorizedOrganization()
        {
            var mockSet = CreateOrganizationsInMockDataStore();

            var mockContext = new Mock<AllReadyContext>();
            mockContext.Setup(c => c.Organizations).Returns(mockSet.Object);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(AllReady.Security.ClaimTypes.UserType, "OrgAdmin"),
                new Claim(AllReady.Security.ClaimTypes.Organization, _organizationId1.ToString())
            }));

            var service = new SelectListService(mockContext.Object);
            var organizations = service.GetOrganizations(principal).ToList();

            Assert.Equal(1, organizations.Count);
            Assert.Equal(_organizationId1.ToString(), organizations.First().Value);
            Assert.Equal(_organizationName1, organizations.First().Text);
        }

        [Fact]
        public void GetOrganizationsForOrgAdminUserWithNoAssociatedOrgReturnsNoOrganizations()
        {
            var mockSet = CreateOrganizationsInMockDataStore();

            var mockContext = new Mock<AllReadyContext>();
            mockContext.Setup(c => c.Organizations).Returns(mockSet.Object);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(AllReady.Security.ClaimTypes.UserType, "OrgAdmin")
            }));

            var service = new SelectListService(mockContext.Object);
            var organizations = service.GetOrganizations(principal).ToList();

            Assert.Equal(0, organizations.Count);
        }

        [Fact]
        public void GetOrganizationsForSimpleUserReturnsNoOrganizations()
        {
            var mockSet = CreateOrganizationsInMockDataStore();

            var mockContext = new Mock<AllReadyContext>();
            mockContext.Setup(c => c.Organizations).Returns(mockSet.Object);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(AllReady.Security.ClaimTypes.UserType, "User")
            }));

            var service = new SelectListService(mockContext.Object);
            var organizations = service.GetOrganizations(principal).ToList();

            Assert.Equal(0, organizations.Count);
       }

        [Fact]
        public void GetOrganizationsForNoClaimsReturnsNoOrganizations()
        {
            var mockSet = CreateOrganizationsInMockDataStore();

            var mockContext = new Mock<AllReadyContext>();
            mockContext.Setup(c => c.Organizations).Returns(mockSet.Object);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[0]));

            var service = new SelectListService(mockContext.Object);
            var organizations = service.GetOrganizations(principal).ToList();

            Assert.Equal(0, organizations.Count);
        }

        [Fact]
        public void GetSkills()
        {
            var data = new List<Skill>
            {
                new Skill { Id = 1, Description = "description1", Name = "c" },
                new Skill { Id = 2, Description = "description2", Name = "b" },
                new Skill { Id = 3, Description = "description3", Name = "a" },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Skill>>();
            mockSet.As<IQueryable<Skill>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Skill>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Skill>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Skill>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<AllReadyContext>();
            mockContext.Setup(c => c.Skills).Returns(mockSet.Object);

            var service = new SelectListService(mockContext.Object);
            var skills = service.GetSkills().ToList();

            var expected = data.OrderBy(x => x.Name).ToList();
            var notExpected = data.OrderByDescending(x => x.Name).ToList();

            Assert.Equal(3, skills.Count);
            Assert.Equal(expected, skills, new SkillComparer());
            // this check makes sure the equality comparer is functioning
            Assert.NotEqual(notExpected, skills, new SkillComparer());
        }

        private static Mock<DbSet<Organization>> CreateOrganizationsInMockDataStore()
        {
            Mock<DbSet<Organization>> mockSet;

            var data = new List<Organization>
            {
                new Organization
                {
                    Id = _organizationId1,
                    Name = _organizationName1,
                },
                 new Organization
                {
                    Id = 2,
                    Name = "Organization Name 2",
                }
            }.AsQueryable();

            mockSet = new Mock<DbSet<Organization>>();
            mockSet.As<IQueryable<Organization>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Organization>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Organization>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Organization>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
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
                    && x.Name.Equals(y.Name, StringComparison.InvariantCulture)
                    && x.Description.Equals(y.Description, StringComparison.InvariantCulture);
            }

            public int GetHashCode(Skill obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}