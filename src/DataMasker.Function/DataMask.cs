using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
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
            try
            {
                logger.LogInformation("Starting execution...");

                logger.LogDebug("Getting configuration");
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                logger.LogDebug("Getting masking configuration");
                var config = await ReadConfig(configuration, logger);

                await _runner.Execute(config);

                logger.LogInformation("Execution complete.");

                return new OkResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error");
                return new InternalServerErrorResult();
            }
        }

        private async Task<Config> ReadConfig(IConfiguration configuration, ILogger logger)
        {
            var connectionString = configuration.GetConnectionString("StorageAccount");
            var containerName = configuration.GetValue<string>("containerName");
            var blobName = configuration.GetValue<string>("blobName");

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the destination blob client
            logger.LogDebug("Creating blob client...");
            var blobClient = storageAccount.CreateCloudBlobClient();
            logger.LogDebug("Creating container reference...");
            var container = blobClient.GetContainerReference(containerName);
            logger.LogDebug("Creating blob reference...");
            var configBlob = container.GetBlobReference(blobName);
            if (!await configBlob.ExistsAsync())
            {
                logger.LogError("Config file can not be found!");
                throw new FileNotFoundException();
            }
            logger.LogDebug("Reading file...");
            await using var stream = await configBlob.OpenReadAsync();
            logger.LogDebug("Loading config...");
            var config = Config.Load(stream);
            return config;
        }
    }
}