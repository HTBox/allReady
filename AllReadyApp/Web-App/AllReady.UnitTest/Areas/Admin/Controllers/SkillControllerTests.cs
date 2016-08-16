﻿using AllReady.Areas.Admin.Controllers;
using Xunit;
using Moq;
using MediatR;
using AllReady.Areas.Admin.Features.Skills;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AllReady.Models;
using AllReady.Areas.Admin.Features.Organizations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using AllReady.Areas.Admin.ViewModels.Skill;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class SkillControllerTests
    {
        const string _orgName = "Org Name";
        const int _orgAdminOrgId = 1;

        #region Index Tests

        [Fact]
        public async Task SkillIndexForSiteAdminReturnsCorrectViewResult()
        {
            // Arrange
            SkillController controller;
            var mockMediator = MockMediatorSkillListQuery(out controller);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.True((result as ViewResult).ViewData.ContainsKey("Title"));

            var title = (result as ViewResult).ViewData["Title"];
            Assert.Equal("Skills", title);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Once);
        }

        [Fact]
        public async Task SkillIndexForValidOrgAdminReturnsCorrectViewResult()
        {
            // Arrange
            SkillController controller;
            var mockMediator = MockMediatorSkillListQuery(out controller);

            var mockContext = MockControllerContextWithUser(OrgAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert 
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);

            Assert.True(result.ViewData.ContainsKey("Title"));

            var title = result.ViewData["Title"];
            Assert.Equal("Skills - " + _orgName, title);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<OrganizationNameQueryAsync>()), Times.Once);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Once);

            Assert.Equal(2, (result.ViewData.Model as IEnumerable<SkillSummaryViewModel>).ToList().Count());
        }

        [Fact]
        public async Task SkillIndexForOrgAdminWithNoOrgIdReturns401()
        {
            // Arrange
            SkillController controller;
            var mockMediator = MockMediatorSkillListQuery(out controller);

            var mockContext = MockControllerContextWithUser(OrgAdminWithMissingOrgId());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Never);
        }

        [Fact(Skip = "NotImplemented")]
        public void IndexHasHttpGetAttribute()
        {
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task SkillCreateForNonAdminOrUserWithNoOrgIdReturns401()
        {
            // Arrange
            SkillController controller;
            var mockMediator = MockMediatorSkillCreateQuery(out controller);

            var mockContext = MockControllerContextWithUser(OrgAdminWithMissingOrgId());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Create();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Never);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()), Times.Never);
        }

        [Fact]
        public async Task SkillCreateForSiteAdminReturnsCorrectResult()
        {
            // Arrange
            SkillController controller;
            var mockMediator = MockMediatorSkillCreateQuery(out controller);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Create() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Edit", result.ViewName);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Once);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()), Times.Once);

            Assert.Equal(1, (result.ViewData.Model as SkillEditViewModel).OrganizationSelection.ToList().Count());
            Assert.Equal(2, (result.ViewData.Model as SkillEditViewModel).ParentSelection.ToList().Count());
        }

        [Fact]
        public async Task SkillCreateForOrgAdminReturnsCorrectResult()
        {
            // Arrange
            SkillController controller;
            var mockMediator = MockMediatorSkillCreateQuery(out controller);

            var mockContext = MockControllerContextWithUser(OrgAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Create() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Edit", result.ViewName);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Once);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()), Times.Never);

            Assert.Null((result.ViewData.Model as SkillEditViewModel).OrganizationSelection);
            Assert.Equal(2, (result.ViewData.Model as SkillEditViewModel).ParentSelection.ToList().Count());
        }

        [Fact(Skip = "NotImplemented")]
        public void CreateGetHasHttpGetAttribute()
        {
        }

        [Fact]
        public async Task SkillCreatePostForSiteAdminWithValidModelStateReturnsRedirectToAction()
        {
            // Arrange
            SkillController controller;
            var mockMediator = MockMediatorSkillCreateQuery(out controller);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act
            var model = CreateSkillModel();
            var result = await controller.Create(model) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillEditCommandAsync>()), Times.Once);
        }

        [Fact]
        public async Task SkillCreatePostForOrgAdminWithValidModelStateReturnsRedirectToAction()
        {
            // Arrange
            SkillController controller;
            var mockMediator = MockMediatorSkillCreateQuery(out controller);

            var mockContext = MockControllerContextWithUser(OrgAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act
            var model = CreateSkillModel();
            var result = await controller.Create(model) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(_orgAdminOrgId, model.OwningOrganizationId);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillEditCommandAsync>()), Times.Once);
        }

        [Fact]
        public async Task SkillCreatePostForSiteAdminWithInvalidModelStateReturnsViewResult()
        {
            // Arrange
            SkillController controller;
            var mockMediator = MockMediatorSkillCreateQuery(out controller);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            controller.ControllerContext = mockContext.Object;

            controller.ModelState.AddModelError("test", "test");

            // Act
            var model = CreateSkillModel();
            var result = await controller.Create(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Edit", result.ViewName);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Once);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()), Times.Once);

            Assert.Equal(1, (result.ViewData.Model as SkillEditViewModel).OrganizationSelection.ToList().Count());
            Assert.Equal(2, (result.ViewData.Model as SkillEditViewModel).ParentSelection.ToList().Count());
        }

        [Fact]
        public async Task SkillCreatePostForOrgAdminWithInvalidModelStateReturnsViewResult()
        {
            // Arrange
            SkillController controller;
            var mockMediator = MockMediatorSkillCreateQuery(out controller);

            var mockContext = MockControllerContextWithUser(OrgAdmin());
            controller.ControllerContext = mockContext.Object;

            controller.ModelState.AddModelError("test", "test");

            // Act
            var model = CreateSkillModel();
            var result = await controller.Create(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Edit", result.ViewName);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Once);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()), Times.Never);

            Assert.Null((result.ViewData.Model as SkillEditViewModel).OrganizationSelection);
            Assert.Equal(2, (result.ViewData.Model as SkillEditViewModel).ParentSelection.ToList().Count());
        }

        [Fact(Skip = "NotImplemented")]
        public void CreatePostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void CreatePostHasValidateAntiForgeryTokenAttribute()
        {
        }

        private static SkillEditViewModel CreateSkillModel()
        {
            return new SkillEditViewModel
            {
                Name = "Some Name",
                Description = "Some Description"
            };
        }

        #endregion

        #region Edit Tests

        [Fact]
        public async Task SkillEditNoSkillReturns404()
        {
            // Arrange
            const int skillId = 0;
            SkillController controller;
            var mockMediator = MockMediatorSkillEditQueryNullModel(out controller);

            // Act
            var result = await controller.Edit(skillId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillEditQueryAsync>()), Times.Once);
        }

        [Fact]
        public async Task SkilEditForNonAdminUserWithNoOrgIdReturns401()
        {
            // Arrange
            const int skillId = 1;
            SkillController controller;
            var mockMediator = MockMediatorSkillEditQuery(out controller);

            var mockContext = MockControllerContextWithUser(OrgAdminWithMissingOrgId());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Edit(skillId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Never);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()), Times.Never);
        }

        [Fact]
        public async Task SkillEditForSiteAdminReturnsCorrectResult()
        {
            // Arrange
            const int skillId = 1;
            SkillController controller;
            var mockMediator = MockMediatorSkillEditQuery(out controller);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Edit(skillId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Edit", result.ViewName);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Once);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()), Times.Once);

            Assert.Equal(1, (result.ViewData.Model as SkillEditViewModel).OrganizationSelection.ToList().Count());
            Assert.Equal(1, (result.ViewData.Model as SkillEditViewModel).ParentSelection.ToList().Count()); // We expect 1 since we should have removed self from the list at this point
        }

        [Fact]
        public async Task SkillEditForOrgAdminReturnsCorrectResult()
        {
            // Arrange
            const int skillId = 1;
            SkillController controller;
            var mockMediator = MockMediatorSkillEditQuery(out controller, new SkillEditViewModel { Id = 1, Name = "Name", Description = "Description", OwningOrganizationId = _orgAdminOrgId });

            var mockContext = MockControllerContextWithUser(OrgAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Edit(skillId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Edit", result.ViewName);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Once);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()), Times.Never);

            Assert.Null((result.ViewData.Model as SkillEditViewModel).OrganizationSelection);
            Assert.Equal(1, (result.ViewData.Model as SkillEditViewModel).ParentSelection.ToList().Count()); // We expect 1 since we should have removed self from the list at this point
        }

        [Fact]
        public async Task SkillEditForOrgAdminWithWrongOrgIdReturnsCorrectResult()
        {
            // Arrange
            const int skillId = 1;
            SkillController controller;
            var mockMediator = MockMediatorSkillEditQuery(out controller, new SkillEditViewModel { Id = 1, Name = "Name", Description = "Description", OwningOrganizationId = 2 });

            var mockContext = MockControllerContextWithUser(OrgAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Edit(skillId) as UnauthorizedResult;

            // Assert
            Assert.NotNull(result);
            
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Never);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()), Times.Never);
        }

        [Fact(Skip = "NotImplemented")]
        public void EditGetHasHttpGetAttrbitue()
        {
        }

        [Fact]
        public async Task SkillEditPostForSiteAdminWithValidModelStateReturnsRedirectToAction()
        {
            // Arrange
            var model = EditSkillModel();
            SkillController controller;
            var mockMediator = MockMediatorSkillEditQuery(out controller, model);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Edit(model) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillEditCommandAsync>()), Times.Once);
        }

        [Fact]
        public async Task SkillEditPostForOrgAdminWithValidModelStateReturnsRedirectToAction()
        {
            // Arrange
            var model = EditSkillModel();
            SkillController controller;
            var mockMediator = MockMediatorSkillEditQuery(out controller, model);

            var mockContext = MockControllerContextWithUser(OrgAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act            
            var result = await controller.Edit(model) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillEditCommandAsync>()), Times.Once);
        }

        [Fact(Skip = "NotImplemented")]
        public void EditPostHasHttpPostAttrbitue()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditPostHasValidateAntiForgeryTokenttrbitue()
        {
        }

        [Fact]
        public async Task SkillEditPostForSiteAdminWithInvalidModelStateReturnsViewResult()
        {
            // Arrange
            var model = EditSkillModel();
            SkillController controller;
            var mockMediator = MockMediatorSkillEditQuery(out controller, model);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            controller.ControllerContext = mockContext.Object;

            controller.ModelState.AddModelError("test", "test");

            // Act
            var result = await controller.Create(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Edit", result.ViewName);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Once);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()), Times.Once);

            Assert.Equal(1, (result.ViewData.Model as SkillEditViewModel).OrganizationSelection.ToList().Count());
            Assert.Equal(2, (result.ViewData.Model as SkillEditViewModel).ParentSelection.ToList().Count());
        }

        [Fact]
        public async Task SkillEditPostForOrgAdminWithInvalidModelStateReturnsViewResult()
        {
            // Arrange
            var model = EditSkillModel();
            SkillController controller;
            var mockMediator = MockMediatorSkillEditQuery(out controller, model);

            var mockContext = MockControllerContextWithUser(OrgAdmin());
            controller.ControllerContext = mockContext.Object;

            controller.ModelState.AddModelError("test", "test");

            // Act
            var result = await controller.Create(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Edit", result.ViewName);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()), Times.Once);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()), Times.Never);

            Assert.Null((result.ViewData.Model as SkillEditViewModel).OrganizationSelection);
            Assert.Equal(2, (result.ViewData.Model as SkillEditViewModel).ParentSelection.ToList().Count());
        }

        private static SkillEditViewModel EditSkillModel()
        {
            return new SkillEditViewModel
            {
                Name = "Some Name",
                Description = "Some Description",
                OwningOrganizationId = _orgAdminOrgId
            };
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task SkillDeleteNoSkillReturns404()
        {
            // Arrange
            const int skillId = 0;
            SkillController controller;
            var mockMediator = MockMediatorSkillDeleteQueryNullModel(out controller);

            // Act
            var result = await controller.Delete(skillId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillDeleteQueryAsync>()), Times.Once);
        }

        [Fact]
        public async Task SkilllDeleteForNonAdminUserWithNoOrgIdReturns401()
        {
            // Arrange
            const int skillId = 1;
            SkillController controller;
            MockMediatorSkillDeleteQuery(out controller);

            var mockContext = MockControllerContextWithUser(OrgAdminWithMissingOrgId());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Delete(skillId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SkillDeleteForSiteAdminReturnsCorrectResult()
        {
            // Arrange
            const int skillId = 1;
            SkillController controller;
            MockMediatorSkillDeleteQuery(out controller);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Delete(skillId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Delete", result.ViewName);
        }

        [Fact]
        public async Task SkillDeleteForOrgAdminReturnsCorrectResult()
        {
            // Arrange
            const int skillId = 1;
            SkillController controller;
            MockMediatorSkillDeleteQuery(out controller, new SkillDeleteViewModel { HierarchicalName = "A Name", OwningOrganizationId = _orgAdminOrgId });

            var mockContext = MockControllerContextWithUser(OrgAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.Delete(skillId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Delete", result.ViewName);
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteHasHttpGetAttribute()
        {
        }

        [Fact]
        public async Task SkillDeleteConfirmedPostNoSkillReturns404()
        {
            // Arrange
            const int skillId = 0;
            SkillController controller;
            var mockMediator = MockMediatorSkillDeleteQueryNullModel(out controller);

            // Act
            var result = await controller.DeleteConfirmed(skillId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillDeleteQueryAsync>()), Times.Once);
        }

        [Fact]
        public async Task SkillDeleteConfirmedPostFromSiteAdminReturnsRedirectToAction()
        {
            // Arrange
            SkillController controller;
            var mockMediator = MockMediatorSkillDeleteQuery(out controller);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            controller.ControllerContext = mockContext.Object;

            // Act            
            var result = await controller.DeleteConfirmed(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<SkillDeleteCommandAsync>()), Times.Once);
        }
        
        [Fact]
        public async Task SkilllDeleteConfirmedPostForNonAdminUserWithNoOrgIdReturns401()
        {
            // Arrange
            const int skillId = 1;
            SkillController controller;
            MockMediatorSkillDeleteQuery(out controller);

            var mockContext = MockControllerContextWithUser(OrgAdminWithMissingOrgId());
            controller.ControllerContext = mockContext.Object;

            // Act
            var result = await controller.DeleteConfirmed(skillId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteConfirmedHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteConfirmedHasActionNameAttributeWithCorrerctActionName()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteConfirmedHasValidateAntiForgeryTokenAttribute()
        {
        }

        #endregion

        #region Helper Methods

        private static Mock<IMediator> MockMediatorSkillListQuery(out SkillController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>()))
                .Returns(() => Task.FromResult(SummaryListItems()))
                .Verifiable();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationNameQueryAsync>()))
                .Returns(() => Task.FromResult(_orgName))
                .Verifiable();
            controller = new SkillController(mockMediator.Object);
            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorSkillCreateQuery(out SkillController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>())).Returns(() => Task.FromResult(SummaryListItems()))
                .Verifiable();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()))
                .Returns(() => Task.FromResult<IEnumerable<SelectListItem>>(new List<SelectListItem> { new SelectListItem { Text = "Item 1", Value = "1" } }))
            .Verifiable();
            controller = new SkillController(mockMediator.Object);
            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorSkillEditQuery(out SkillController controller, SkillEditViewModel model = null)
        {
            if (model == null) model = new SkillEditViewModel { Id = 1, Name = "Name", Description = "Description" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<SkillEditQueryAsync>())).Returns(() => Task.FromResult(model)).Verifiable();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<SkillListQueryAsync>())).Returns(() => Task.FromResult(SummaryListItems()))
                .Verifiable();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationSelectListQueryAsync>()))
                .Returns(() => Task.FromResult<IEnumerable<SelectListItem>>(new List<SelectListItem> { new SelectListItem { Text = "Item 1", Value = "1" } }))
                .Verifiable();
            controller = new SkillController(mockMediator.Object);
            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorSkillEditQueryNullModel(out SkillController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<SkillEditQueryAsync>())).Returns(() => Task.FromResult<SkillEditViewModel>(null)).Verifiable();
            controller = new SkillController(mockMediator.Object);
            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorSkillDeleteQuery(out SkillController controller, SkillDeleteViewModel model = null)
        {
            if (model == null) model = new SkillDeleteViewModel {  HierarchicalName = "Name" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<SkillDeleteQueryAsync>())).Returns(() => Task.FromResult(model)).Verifiable();
            controller = new SkillController(mockMediator.Object);
            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorSkillDeleteQueryNullModel(out SkillController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<SkillDeleteQueryAsync>())).Returns(() => Task.FromResult<SkillDeleteViewModel>(null)).Verifiable();
            controller = new SkillController(mockMediator.Object);
            return mockMediator;
        }

        private static IEnumerable<SkillSummaryViewModel> SummaryListItems()
        {
            return new List<SkillSummaryViewModel>
            {
                new SkillSummaryViewModel { Id = 1, HierarchicalName = "Name", OwningOrganizationName = "Org" },
                new SkillSummaryViewModel { Id = 2, HierarchicalName = "Name 2", OwningOrganizationName = "Org" }
            };
        }

        private static Mock<ControllerContext> MockControllerContextWithUser(ClaimsPrincipal principle)
        {
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User).Returns(() => principle);
            var mockContext = new Mock<ControllerContext>();

            mockContext.Object.HttpContext = mockHttpContext.Object;
            return mockContext;
        }

        private static ClaimsPrincipal OrgAdminWithMissingOrgId()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString())
            }));
        }

        private static ClaimsPrincipal OrgAdmin()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, _orgAdminOrgId.ToString())
            }));
        }

        private static ClaimsPrincipal SiteAdmin()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString())
            }));
        }

        #endregion
    }
}