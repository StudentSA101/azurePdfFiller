using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using iTextSharp.text.pdf;

namespace Pdf
{
    public static class PdfFiller
    {
        [FunctionName("document-render")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            ILogger log)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string pdfTemplate = @"c:\template.pdf";
            string newFile = @"c:\projects\PDF\completed_fw5.pdf";
            PdfReader pdfReader = new PdfReader(pdfTemplate);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;

            log.LogInformation("C# HTTP trigger function populating pdf commencing.");

            pdfFormFields.SetField("date", data.date.ToString());
            pdfFormFields.SetField("name", data.insured.ToString());
            pdfFormFields.SetField("surname", data.insured_identity.ToString());
            pdfFormFields.SetField("email", data.owner.ToString());

            log.LogInformation("C# HTTP trigger function populating fields finalised");

            pdfStamper.FormFlattening = true;

            log.LogInformation("C# HTTP trigger function flattened pdf");

            pdfStamper.Close();

            log.LogInformation("C# HTTP trigger function flattened pdf");

            return data != null
                ? (ActionResult)new OkObjectResult($"C# HTTP trigger function successfully rendered PDF")
                : new BadRequestObjectResult("Please pass data in the request body");
        }
    }
}
