using System.Collections.Generic;
using System.Text;
using DataMasker.Utils;
using Microsoft.Extensions.Logging;

namespace DataMasker.Function
{
    public class ProgressUpdater : IProgressUpdater
    {
        private readonly ILogger _logger;
        private readonly Progress _progress;

        public ProgressUpdater(ILogger<ProgressUpdater> logger)
        {
            _logger = logger;
            _progress = new Progress(0, "Overall progress");
        }
        
        public void UpdateProgress(ProgressType progressType, int current, int? max = null, string message = null)
        {
            if (progressType != ProgressType.Overall)
            {
                return;
            }
            _progress.Max = max ?? _progress.Max;
            _progress.Message = message ?? _progress.Message;
            _progress.Current = current;

            _logger.LogInformation($"{_progress.Message}: {_progress.Percentage:P}\t");

        }
        
        private class Progress
        {
            public Progress(int max, string message)
            {
                Max = max;
                Message = message;
            }
            
            public int Max { get; set; }

            public int Current { get; set; }

            public decimal Percentage
            {
                get
                {
                    if (Max == 0) return 0;
                    return (decimal)Current / Max;
                }
            }

            public string Message { get; set; }
        }
    }
}