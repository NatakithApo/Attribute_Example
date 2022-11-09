using CMU.Budget.API.Enums;
using CMU.Budget.API.Exceptions.HttpExceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;

namespace CMU.Budget.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RolePermission : ActionFilterAttribute
    {
        private RoleEnum _role;

        public RolePermission(RoleEnum role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (context.HttpContext == null || context.HttpContext.User == null)
                throw new UnauthorizedException();

            ClaimsPrincipal user = context.HttpContext.User;
            string sRoleId = user.FindFirst("roleId")?.Value;
            int roleId = 0;

            if (string.IsNullOrEmpty(sRoleId) || !int.TryParse(sRoleId, out roleId) || roleId == 0)
            {
                throw new HttpInternalServerErrorException("Unable to parse the role string. The role is either NULL or the format is invalid.");
            }

            if (roleId > _role.GetHashCode())
            {
                throw new HttpMethodNotAllowedException("Permission denied.");
            }
        }
    }
}
