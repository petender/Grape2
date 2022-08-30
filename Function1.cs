using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using GrapeCity.Documents.Imaging;
using System.ComponentModel;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using System.Drawing;
using System.Xml.Linq;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //GcBitmap.SetLicenseKey();

            try
            {
                var file = req.Form.Files[0];
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    ms.Position = 0;
                    using (var image = new GcBitmap(ms))
                    {
                        int w, h;
                        if (image.Width >= image.Height)
                        {
                            w = 128;
                            h = (int)Math.Round(128 * image.Height / image.Width);
                        }
                        else
                        {
                            h = 128;
                            w = (int)Math.Round(128 * image.Width / image.Height);
                        }
                        var msRet = new MemoryStream();
                        using (var thumbnail = image.Resize(w, h, InterpolationMode.Downscale))
                        {
                            thumbnail.SaveAsJpeg(msRet);
                            msRet.Position = 0;
                            var result = new FileStreamResult(msRet, "image/jpeg");
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Error:\n{ex.Message}");
            }
        }
    }
}
IMPO