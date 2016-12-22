using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class UnlinkedRequestsController : Controller
    {
        private readonly IMediator _mediator;

        public UnlinkedRequestsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Admin/UnlinkedRequests/List
        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            if (!User.IsOrganizationAdmin())
            {
                return Unauthorized();
            }

            var orgId = User.GetOrganizationId();

            var viewModel = new UnlinkedRequestsViewModel()
            {
                Requests =
                    await
                        _mediator.SendAsync(new RequestListItemsQuery()
                        {
                            Criteria = new RequestSearchCriteria() { OrganizationId = orgId, Status = RequestStatus.Unassigned }
                        })
            };

            return View(viewModel);
        }
    }
}
