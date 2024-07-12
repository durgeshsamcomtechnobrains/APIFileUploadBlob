using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System;

public class CheckImageFunction
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "files";

    public CheckImageFunction(IConfiguration configuration)
    {
        //creadentials
        string accountName = configuration["StorageAccountName"];
        string accountKey = configuration["StorageAccountKey"];
        var credentials = new StorageSharedKeyCredential(accountName, accountKey);
        var blobUri = $"https://{accountName}.blob.core.windows.net";
        _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credentials);
    }

    //This functions check if image is exist in blob or not
    [FunctionName("CheckImageFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "CheckImage/{imageName}")] HttpRequest req,
        string imageName,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(imageName);

        //if exist its return us TRUE (image Test exist)
        if (await blobClient.ExistsAsync())
        {
            log.LogInformation($"Image '{imageName}' exists.");
            return new OkObjectResult($"Image '{imageName}' exists.");
        }
        //if now return FALSE
        else
        {
            log.LogInformation($"Image '{imageName}' does not exist.");
            return new NotFoundObjectResult($"Image '{imageName}' does not exist.");
        }
    }
}