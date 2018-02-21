using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Features.Goals;
using AllReady.Areas.Admin.ViewModels.Goal;
using AllReady.Constants;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize]
    public class GoalController : Controller
    {
        private readonly IMediator _mediator;

        public GoalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET /Admin/Goal/Create/{campaignId}
        [HttpGet]
        public async Task<IActionResult> Create(int campaignId)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery {CampaignId = campaignId});
            if (campaign == null) 
            {
                return NotFound();
            }

            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(campaign.Id));
            if (!await authorizableCampaign.UserCanEdit())
            {
                return Unauthorized();
            }

            var viewModel = new GoalEditViewModel()
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name,
                OrganizationId = campaign.OrganizationId,
                Display = true,
                GoalType = GoalType.Numeric
            };

            return View("Edit", viewModel);
        }

        // POST /Admin/Goal/Create/{campaignId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int campaignId, GoalEditViewModel model)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery {CampaignId = campaignId});
            if (campaign == null) 
            {
                return NotFound();
            }

            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(campaign.Id));
            if (!await authorizableCampaign.UserCanEdit())
            {
                return Unauthorized();
            }
            
            ValidateGoalEditViewModel(model);
            if (ModelState.IsValid)
            {
                await _mediator.SendAsync(new GoalEditCommand {Goal = model});
                return RedirectToAction(nameof(CampaignController.Details), "Campaign", new {area = AreaNames.Admin, id = campaignId});
            }
            return View("Edit", model);
        }

        // GET /Admin/Goal/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var goal = await _mediator.SendAsync(new GoalDeleteQuery {GoalId = id});
            if (goal == null)
            {
                return NotFound();
            }

            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(goal.CampaignId));
            if (!await authorizableCampaign.UserCanDelete())
            {
                return Unauthorized();
            }

            return View("Delete", goal);
        }
        
        // GET /Admin/Skill/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var goal = await _mediator.SendAsync(new GoalDeleteQuery {GoalId = id});
            if (goal == null)
            {
                return NotFound();
            }

            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(goal.CampaignId));
            if (!await authorizableCampaign.UserCanDelete())
            {
                return Unauthorized();
            }

            await _mediator.SendAsync(new GoalDeleteCommand {GoalId = id});
            return RedirectToAction(nameof(CampaignController.Details), "Campaign", new {area = AreaNames.Admin, id = goal.CampaignId});
        }

        // GET /Admin/Goal/Edit/{goalId}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var goal = await _mediator.SendAsync(new GoalEditQuery {GoalId = id});
            if (goal == null)
            {
                return NotFound();
            }

            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(goal.CampaignId));
            if (!await authorizableCampaign.UserCanEdit())
            {
                return Unauthorized();
            }

            var viewModel = new GoalEditViewModel
            {
                CampaignId = goal.CampaignId,
                CampaignName = goal.CampaignName,
                Id = id,
                CurrentGoalLevel = goal.CurrentGoalLevel,
                Display = goal.Display,
                GoalType = goal.GoalType,
                TextualGoal = goal.TextualGoal,
                NumericGoal = goal.NumericGoal,
                OrganizationId = goal.OrganizationId
            };

            return View("Edit", viewModel);
        }

        // POST /Admin/Goal/Edit/{goalId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GoalEditViewModel model)
        {
            var goal = await _mediator.SendAsync(new GoalEditQuery {GoalId = id});
            if (goal == null)
            {
                return NotFound();
            }

            if (id != model.Id)
            {
                return BadRequest();
            }

            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(goal.CampaignId));
            if (!await authorizableCampaign.UserCanEdit())
            {
                return Unauthorized();
            }

            ValidateGoalEditViewModel(model);

            if (ModelState.IsValid)
            {
                await _mediator.SendAsync(new GoalEditCommand {Goal = model});
                return RedirectToAction(nameof(CampaignController.Details), "Campaign", new {area = AreaNames.Admin, id = goal.CampaignId});
            }
            return View("Edit", model);
        }

        private void ValidateGoalEditViewModel(GoalEditViewModel model)
        {
            if (model.GoalType == GoalType.Numeric)
            {
                if (model.NumericGoal <= 0)
                    ModelState.AddModelError(nameof(model.NumericGoal), "The field Numeric Goal must be greater than 0");
                if (model.CurrentGoalLevel < 0)
                    ModelState.AddModelError(nameof(model.CurrentGoalLevel),
                        "The field Current Goal Level must be greater than or equal to 0");
            }
            else
            {
                if (string.IsNullOrEmpty(model.TextualGoal))
                    ModelState.AddModelError(nameof(model.TextualGoal),
                        "The Campaign Goal field is required for a Textual Goal");
            }
        }
    }
}
