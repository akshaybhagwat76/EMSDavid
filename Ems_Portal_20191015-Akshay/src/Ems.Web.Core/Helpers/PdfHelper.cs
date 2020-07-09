using PDFFactoryServiceReference;
using System.Threading.Tasks;

namespace Ems.Web.Helpers
{
    public class PdfHelper
    {
        public async Task<string> GeneratePdf(string htmlToConvert, string baseUrl, string pdfFileName)
        {
            string result = "failed";
           // baseUrl = "devandpreproduction";
            PdfFactoryClient client = new PdfFactoryClient();

            PdfRequestInfo pdfRequestInfo = new PdfRequestInfo()
            {
                Filename = pdfFileName,
                BaseUrl = baseUrl,
                Html = htmlToConvert
            };

            result = await client.CindiGeneratePdfAsync(pdfRequestInfo);

            await client.CloseAsync();

            return result;
        }
    }
}
