using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.UnlinkedRequests;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class UnlinkedRequestController : Controller
    {
        private readonly IMediator _mediator;

        public UnlinkedRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Admin/UnlinkedRequest/List
        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            var orgId = User.GetOrganizationId();
            if (!User.IsOrganizationAdmin())
            {
                return Unauthorized();
            }

            return View(await _mediator.SendAsync(new UnlinkedRequestListQuery()
            {
                OrganizationId = orgId.GetValueOrDefault()
            }));
        }
    }
}