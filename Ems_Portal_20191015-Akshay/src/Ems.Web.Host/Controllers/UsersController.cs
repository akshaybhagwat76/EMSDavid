using Abp.AspNetCore.Mvc.Authorization;
using Ems.Authorization;
using Ems.Storage;
using Abp.BackgroundJobs;

namespace Ems.Web.Controllers
{
    [AbpMvcAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UsersController : UsersControllerBase
    {
        public UsersController(IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager)
            : base(binaryObjectManager, backgroundJobManager)
        {
        }
    }
}