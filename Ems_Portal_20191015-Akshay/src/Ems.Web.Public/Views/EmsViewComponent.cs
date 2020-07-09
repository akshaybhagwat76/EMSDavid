using Abp.AspNetCore.Mvc.ViewComponents;

namespace Ems.Web.Public.Views
{
    public abstract class EmsViewComponent : AbpViewComponent
    {
        protected EmsViewComponent()
        {
            LocalizationSourceName = EmsConsts.LocalizationSourceName;
        }
    }
}