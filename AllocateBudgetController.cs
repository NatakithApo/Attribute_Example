using CMU.Budget.API.Attributes;
using CMU.Budget.API.DataTransferObjects;
using CMU.Budget.API.DataTransferObjects.AllocateBudget;
using CMU.Budget.API.DataTransferObjects.Budget;
using CMU.Budget.API.DataTransferObjects.Log;
using CMU.Budget.API.DataTransferObjects.Planning;
using CMU.Budget.API.Services;
using CMU.Budget.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CMU.Budget.API.Controllers
{
    [ApiController]
    [TrackRequestLog]
    [Authorize]
    [Route("api/v1/allocate-budget")]
    public class AllocateBudgetController : ControllerBase
    {
        private readonly AllocateBudgetService _allocateBudgetService;
        private readonly CmuPlanningService _cmuPlanningService;
        private readonly LogService _logService;

        public AllocateBudgetController(AllocateBudgetService allocateBudgetService, CmuPlanningService cmuPlanningService, LogService logService)
        {
            _allocateBudgetService = allocateBudgetService;
            _cmuPlanningService = cmuPlanningService;
            _logService = logService;
        }

        [HttpGet("")]
        [RolePermission(Enums.RoleEnum.STAFF)]
        public AllocateBudgetResponseDto GetAllocateBudget(int organizationId, int year)
        {
            return _allocateBudgetService.GetAllocateBudget(organizationId, year);
        }

        [HttpPost("")]
        [RolePermission(Enums.RoleEnum.ADMIN)]
        public AllocateBudgetResponseDto UpdateAllocateBudget([FromBody]AllocateBudgetRequestDto model)
        {
            return _allocateBudgetService.UpdateAllocateBudget(model, UserHelper.GetUserIdFromPayload(User));
        }

        [HttpPost("chart/submit")]
        [RolePermission(Enums.RoleEnum.ADMIN)]
        public AllocateBudgetResponseDto SubmitOrganizationChart([FromBody] AllocateBudgetRequestDto model)
        {
            return _allocateBudgetService.SubmittAllocateBudget(model.OrganizationId, model.BudgetYear, UserHelper.GetUserIdFromPayload(User));
        }

        [HttpPost("chart/review")]
        [RolePermission(Enums.RoleEnum.APPROVER)]
        public AllocateBudgetResponseDto ReviewOrganizationChart([FromBody] ApprovalRequestDto model)
        {
            return _allocateBudgetService.ReviewAllocateBudget(model.OrganizationId, model.BudgetYear, UserHelper.GetUserIdFromPayload(User), model);
        }

        [HttpPost("chart/unfreeze")]
        [RolePermission(Enums.RoleEnum.APPROVER)]
        public AllocateBudgetResponseDto UnfreezeOrganizationChart([FromBody] ApprovalRequestDto model)
        {
            return _allocateBudgetService.UnfreezeAllocateBudget(model.OrganizationId, model.BudgetYear, UserHelper.GetUserIdFromPayload(User), model.Reason);
        }

        [HttpGet("reference")]
        [RolePermission(Enums.RoleEnum.STAFF)]
        public BudgetReferenceDto GetBudgetReference(int budgetyear)
        {
            return _allocateBudgetService.GetBudgetReference(budgetyear);
        }

        [HttpGet("budget-3ds")]
        [RolePermission(Enums.RoleEnum.STAFF)]
        public List<Budget3DDto> GetBudget3D(int organizationId, int budgetYear, int? fundId, int? projectId, int? typeId, int? strategyId)
        {
            return _allocateBudgetService.GetBudget3D(organizationId, budgetYear, fundId, projectId, typeId, strategyId);
        }

        [AllowAnonymous]
        [HttpGet("planning")]
        public double GetPlanningBudget(int budgetYear)
        {
            return _cmuPlanningService.GetCmuPlanningBudget(budgetYear);
        }

        [HttpGet("logs")]
        public List<LogDto> GetLog([FromQuery] int organizationId, [FromQuery] int year)
        {
            return _logService.GetAllocateBudgetLogs(organizationId,year);
        }
    }
}
