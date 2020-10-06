using System.Collections.Generic;
using DataMasker.Utils;
using Konsole;

namespace DataMasker.Cli
{
    public class ProgressUpdater : IProgressUpdater
    {
        private readonly Dictionary<ProgressType, ProgressbarUpdate> _progressBars;

        public ProgressUpdater()
        {
            _progressBars = new Dictionary<ProgressType, ProgressbarUpdate>
            {
                {
                    ProgressType.Overall,
                    new ProgressbarUpdate
                    {
                        ProgressBar = new ProgressBar(PbStyle.SingleLine, 0), LastMessage = "Overall Progress"
                    }
                },
                {
                    ProgressType.Updating,
                    new ProgressbarUpdate
                    {
                        ProgressBar = new ProgressBar(PbStyle.SingleLine, 0), LastMessage = "Update Progress"
                    }
                },
                {
                    ProgressType.Masking,
                    new ProgressbarUpdate
                    {
                        ProgressBar = new ProgressBar(PbStyle.SingleLine, 0), LastMessage = "Masking Progress"
                    }
                }
            };
        }

        public void UpdateProgress(ProgressType progressType, int current, int? max = null, string message = null)
        {
            max ??= _progressBars[progressType]
                .ProgressBar.Max;

            _progressBars[progressType]
                .ProgressBar.Max = max.Value;

            message ??= _progressBars[progressType]
                .LastMessage;

            _progressBars[progressType]
                .ProgressBar.Refresh(current, message);
        }
    }
}