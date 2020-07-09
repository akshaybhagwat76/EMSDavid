using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using Ems.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ems.Web.Controllers
{
    [AbpMvcAuthorize]
    public class EmsPdfController : EmsControllerBase
    {
        private static PdfHelper _pdfService;

        public EmsPdfController()
        {
            _pdfService = new PdfHelper();
        }

        [HttpPost]
        [DisableAuditing]
        public async Task<string> GeneratePdf(string baseUrl, string pdfFileName)
        {
            try
            {
                if (Request.Form.Keys.Contains("htmlString"))
                {
                    string htmlToConvert = Request.Form["htmlString"].ToString();

                    string generatedUrl = await _pdfService.GeneratePdf(htmlToConvert, baseUrl, pdfFileName);

                    return generatedUrl;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}