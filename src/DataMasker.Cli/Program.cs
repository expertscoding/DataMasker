using System.Collections.Generic;
using System.Threading.Tasks;
using DataMasker.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DataMasker.Cli
{
    class Program
    {
        private static readonly Dictionary<ProgressType, ProgressbarUpdate> ProgressBars = new Dictionary<ProgressType, ProgressbarUpdate>();
        
        static async Task Main(string[] args)
        {
            
            var services = new ServiceCollection();
            ConfigureServices(services);
            await using var serviceProvider = services.BuildServiceProvider();
            var app = serviceProvider.GetService<Application>();
            
            await app.Run(args);
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging(configure =>configure.AddConsole())
                .AddTransient<Application>()
                .AddTransient<IProgressUpdater, ProgressUpdater>()
                .AddTransient<Runner>();
        }
    }
}