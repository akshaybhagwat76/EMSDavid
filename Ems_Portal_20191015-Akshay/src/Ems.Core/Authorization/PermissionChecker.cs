using Abp.Authorization;
using Ems.Authorization.Roles;
using Ems.Authorization.Users;

namespace Ems.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
