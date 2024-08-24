using Azure.Storage.Blobs;
using AzureFlexStorage.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureFlexStorage
{
    public static class UploadToBlob
    {
        [FunctionName("UploadToBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Blob("samples-workitems/{name}", FileAccess.Write, Connection = "AzureWebJobsStorage")] BlobClient blobClient,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            using (var stream = new MemoryStream())
            {
                await req.Body.CopyToAsync(stream);
                stream.Position = 0;
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            var response = new
            {
                Message = Messages.FileUploadSuccess,
                FileName = blobClient.Name,
                BlobUri = blobClient.Uri,
                UploadTime = DateTime.UtcNow
            };

            return new OkObjectResult(response);
        }
    }
}
