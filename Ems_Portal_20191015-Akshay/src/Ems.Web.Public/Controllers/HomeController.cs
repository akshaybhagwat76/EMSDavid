using Microsoft.AspNetCore.Mvc;
using Ems.Web.Controllers;

namespace Ems.Web.Public.Controllers
{
    public class HomeController : EmsControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}