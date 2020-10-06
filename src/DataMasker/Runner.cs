using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMasker.Interfaces;
using DataMasker.Models;
using DataMasker.Utils;
using Microsoft.Extensions.Logging;

namespace DataMasker
{
    public class Runner
    {
        private readonly IProgressUpdater _progressUpdater;
        private readonly ILogger _logger;
        
        public Runner(ILoggerFactory loggerFactory, IProgressUpdater progressUpdater)
        {
            _logger = loggerFactory.CreateLogger<Runner>();
            _progressUpdater = progressUpdater;
        }
        
        public async Task Execute(
            Config config)
        {
            _logger.LogInformation("Masking Data");
            _progressUpdater.UpdateProgress(ProgressType.Overall, 0, config.Tables.Count, "Overall Progress");

            //create a data masker
            IDataMasker dataMasker = new DataMasker(new DataGenerator(config.DataGeneration));

            //grab our dataSource from the config, note: you could just ignore the config.DataSource.Type
            //and initialize your own instance
            IDataSource dataSource = DataSourceProvider.Provide(config.DataSource.Type, config.DataSource);

            for (int i = 0; i < config.Tables.Count; i++)
            {
                TableConfig tableConfig = config.Tables[i];

                var rowCount = await dataSource.GetCountAsync(tableConfig);
                _progressUpdater.UpdateProgress(ProgressType.Masking, 0, rowCount, "Masking Progress");
                _progressUpdater.UpdateProgress(ProgressType.Updating, 0, rowCount, "Update Progress");

                IEnumerable<IDictionary<string, object>> rows =await dataSource.GetDataAsync(tableConfig);

                var maskedRows = rows.Select((row,rowIndex) =>
                {
                    _progressUpdater.UpdateProgress(ProgressType.Masking, rowIndex + 1);
                    return dataMasker.Mask(row, tableConfig);
                });

                //update all rows
                await dataSource.UpdateRowsAsync(maskedRows, rowCount, tableConfig,
                    totalUpdated => _progressUpdater.UpdateProgress(ProgressType.Updating, totalUpdated));
                _progressUpdater.UpdateProgress(ProgressType.Overall, i + 1);
            }

            _logger.LogInformation("Done");
        }
    }
}