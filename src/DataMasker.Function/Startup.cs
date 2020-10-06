using DataMasker.Utils;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DataMasker.Function.Startup))]

namespace DataMasker.Function
{
    public class Startup: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddLogging()
                .AddTransient<IProgressUpdater, ProgressUpdater>()
                .AddTransient<Runner>();
        }
    }
}