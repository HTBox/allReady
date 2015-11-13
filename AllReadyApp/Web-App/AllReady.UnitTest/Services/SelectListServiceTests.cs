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
        public void GetTenantsTest()
        {
            const int tenantId1 = 1;
            const string tenantName1 = "Tenant Name 1";

            var data = new List<Tenant>
            {
                new Tenant
                {
                    Id = tenantId1,
                    Name = tenantName1,
                },
                 new Tenant
                {
                    Id = 1,
                    Name = "Tenant Name 2",
                }
            }.AsQueryable();


            var mockSet = new Mock<DbSet<Tenant>>();
            mockSet.As<IQueryable<Tenant>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Tenant>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Tenant>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Tenant>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<AllReadyContext>();
            mockContext.Setup(c => c.Tenants).Returns(mockSet.Object);

            var service = new SelectListService(mockContext.Object);
            var tenants = service.GetTenants().ToList();

            Assert.Equal(2, tenants.Count);
            Assert.Equal(tenantId1.ToString(), tenants.First().Value);
            Assert.Equal(tenantName1, tenants.First().Text);
         
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
