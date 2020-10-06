namespace DataMasker.Utils
{
    public interface IProgressUpdater
    {
        void UpdateProgress(ProgressType progressType, int current, int? max = null, string message = null);
    }
}