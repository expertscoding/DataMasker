using System;
using System.IO;
using System.Threading.Tasks;
using DataMasker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace DataMasker.Function
{
    public class DataMask
    {
        private readonly Runner _runner;

        public DataMask(Runner runner)
        {
            _runner = runner;
        }
        
        [FunctionName("DataMask")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req, ILogger logger, ExecutionContext context)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            
            var config = await ReadConfig(configuration);

            await _runner.Execute(config);
            
            return new OkResult();
            
        }

        private static async Task<Config> ReadConfig(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("StorageAccount");
            var containerName = configuration.GetValue<string>("containerName");
            var blobName = configuration.GetValue<string>("blobName");

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the destination blob client
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var configBlob = container.GetBlobReference(blobName);

            await using var stream = await configBlob.OpenReadAsync();
            var config = Config.Load(stream);
            return config;
        }
    }
}