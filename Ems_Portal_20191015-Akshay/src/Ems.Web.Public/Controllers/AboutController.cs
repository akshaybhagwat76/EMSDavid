using Microsoft.AspNetCore.Mvc;
using Ems.Web.Controllers;

namespace Ems.Web.Public.Controllers
{
    public class AboutController : EmsControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}