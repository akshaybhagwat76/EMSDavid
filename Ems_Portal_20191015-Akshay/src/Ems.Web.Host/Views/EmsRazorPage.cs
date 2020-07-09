using Abp.AspNetCore.Mvc.Views;

namespace Ems.Web.Views
{
    public abstract class EmsRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected EmsRazorPage()
        {
            LocalizationSourceName = EmsConsts.LocalizationSourceName;
        }
    }
}
