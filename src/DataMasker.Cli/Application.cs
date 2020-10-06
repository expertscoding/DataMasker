using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using DataMasker.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DataMasker.Cli
{
    public class Application
    {
        
        private Options _cliOptions;
        private readonly Runner _runner;
        private readonly ILogger _logger;

        public Application(Runner runner, ILogger<Application> logger)
        {
            _runner = runner;
            _logger = logger;
        }

        public async Task Run(IEnumerable<string> args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(
                    async options =>
                    {
                        _cliOptions = options;
                        try
                        {
                            await RuntimeArgumentHandle();
                        }
                        catch (Exception ex)
                        {
                            WriteLine(ex.Message);
                        }
                    });
        }
        
        
        private async Task RuntimeArgumentHandle()
        {
            if (_cliOptions.PrintOptions)
            {
                WriteLine();
                WriteLine(JsonConvert.SerializeObject(_cliOptions, Formatting.Indented));
                WriteLine();
                return;
            }

            var config = Config.Load(_cliOptions.ConfigFile);
            if (_cliOptions.DryRun != null)
            {
                config.DataSource.DryRun = _cliOptions.DryRun.Value;
            }

            if (!string.IsNullOrEmpty(_cliOptions.Locale))
            {
                config.DataGeneration.Locale = _cliOptions.Locale;
            }

            if (_cliOptions.UpdateBatchSize != null)
            {
                config.DataSource.UpdateBatchSize = _cliOptions.UpdateBatchSize;
            }

            await _runner.Execute(config);
        }
        
        
        private void WriteLine(
            string message = null)
        {
            if (!_cliOptions.NoOutput)
            {
                _logger.LogInformation(message);
            }
        }
    }
}