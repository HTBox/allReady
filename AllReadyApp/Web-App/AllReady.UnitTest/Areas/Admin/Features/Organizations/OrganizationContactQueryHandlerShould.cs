using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.ViewModels.OrganizationApi;
using AllReady.Models;
using Shouldly;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class OrganizationContactQueryHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var htb = new Organization
            {
                Id = 1,
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Location = new Location { Id = 1},
                OrganizationContacts = new List<OrganizationContact>
                {
                    new OrganizationContact
                    {
                        ContactType = (int) ContactTypes.Primary,
                        Contact = new Contact {FirstName = "Regina"}
                    }
                },
                Campaigns = new List<Campaign>()
            };

            Context.Organizations.Add(htb);
            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnUnpopulatedContactInformationViewModelIfOrganizationIsNull()
        {
            var sut = new OrganizationContactQueryHandler(Context);

            var message = new OrganizationContactQuery {OrganizationId = 2};

            var result = await sut.Handle(message);

            result.ShouldNotBeNull();
            result.ShouldBeOfType<ContactInformationViewModel>();

            var contactInfo = new ContactInformationViewModel();
            Assert.True(result.FirstName == contactInfo.FirstName &&
                        result.LastName == contactInfo.LastName &&
                        result.Email == contactInfo.Email &&
                        result.Location == contactInfo.Location &&
                        result.PhoneNumber == contactInfo.PhoneNumber);
        }


        [Fact]
        public async Task ReturnTheCorrectContactInformationViewModel()
        {
            var sut = new OrganizationContactQueryHandler(Context);

            var message = new OrganizationContactQuery { OrganizationId = 1 };

            var result = await sut.Handle(message);

            var organization = await Context.Organizations
                .AsNoTracking()
                .Include(l => l.Location)
                .Include(oc => oc.OrganizationContacts).ThenInclude(c => c.Contact)
                .SingleOrDefaultAsync(o => o.Id == message.OrganizationId);

            var contact = organization.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<ContactInformationViewModel>();
            result.Location.Id.ShouldBe(organization.Location.ToModel().Id);

            Assert.True(result.Email == contact.Email &&
                        result.FirstName == contact.FirstName &&
                        result.LastName == contact.LastName &&
                        result.PhoneNumber == contact.PhoneNumber);
        }


    }
}
