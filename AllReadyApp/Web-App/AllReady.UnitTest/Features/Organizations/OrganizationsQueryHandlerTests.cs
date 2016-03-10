using System.Collections.Generic;
using AllReady.Features.Organizations;
using AllReady.Models;
using AllReady.ViewModels;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Organizations
{
    public class OrganizationsQueryHandlerTests
    {
        [Fact]
        public void HandleReturnsAllOrganizations()
        {
            var message = new OrganizationsQuery();
            var organizations = new List<Organization>
            {
                new Organization { Id = 1 },
                new Organization { Id = 2 },
            };

            var mockedDataAccess = new Mock<IAllReadyDataAccess>();
            mockedDataAccess.Setup(x => x.Organizations).Returns(organizations);

            var sut = new OrganizationsQueryHandler(mockedDataAccess.Object);
            var results = sut.Handle(message);

            Assert.Equal(results[0].Id, organizations[0].Id);
            Assert.Equal(results[1].Id, organizations[1].Id);
        }

        [Fact]
        public void HandleReturnsListOfOrganizationViewModels()
        {
            var message = new OrganizationsQuery();
            var sut = new OrganizationsQueryHandler(Mock.Of<IAllReadyDataAccess>());
            var results = sut.Handle(message);

            Assert.IsType<List<OrganizationViewModel>>(results);
        }
    }
}
