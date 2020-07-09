using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace Ems.Web.Public.Views
{
    public abstract class EmsRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected EmsRazorPage()
        {
            LocalizationSourceName = EmsConsts.LocalizationSourceName;
        }
    }
}
