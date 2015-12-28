using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.Services;
using Microsoft.Data.Entity;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Services
{
    public class SelectListServiceTests
    {

        [Fact]
        public void GetOrganizationsTest()
        {
            const int organizationId1 = 1;
            const string OrganizationName1 = "Organization Name 1";

            var data = new List<Organization>
            {
                new Organization
                {
                    Id = organizationId1,
                    Name = OrganizationName1,
                },
                 new Organization
                {
                    Id = 1,
                    Name = "Organization Name 2",
                }
            }.AsQueryable();


            var mockSet = new Mock<DbSet<Organization>>();
            mockSet.As<IQueryable<Organization>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Organization>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Organization>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Organization>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<AllReadyContext>();
            mockContext.Setup(c => c.Organizations).Returns(mockSet.Object);

            var service = new SelectListService(mockContext.Object);
            var organizations = service.GetOrganizations().ToList();

            Assert.Equal(2, organizations.Count);
            Assert.Equal(organizationId1.ToString(), organizations.First().Value);
            Assert.Equal(OrganizationName1, organizations.First().Text);
         
        }

        [Fact]
        public void GetSkillsTest()
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
