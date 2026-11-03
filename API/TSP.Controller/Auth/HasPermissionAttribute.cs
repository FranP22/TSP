using Microsoft.AspNetCore.Authorization;
using TSP.Common.Enum;

namespace TSP.Controller.Auth
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(RolePermission permission) 
        {
            Policy = $"Permission:{permission}";
        }
    }
}
