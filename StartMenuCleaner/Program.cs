namespace StartMenuCleaner
{
    using CommandLine;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using System;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Start Menu Cleaner";

            Parser.Default.ParseArguments<ProgramOptions>(args).WithParsed(Run);
        }

        private static void Run(ProgramOptions options)
        {
            IServiceProvider services = ConfigureServices(options);
            ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Starting");

            if (options.Debug)
            {
                SerilogLogging.SetMinLogLevel(Serilog.Events.LogEventLevel.Debug);
                logger.LogInformation("Debug logging is enabled");
            }

            Console.WriteLine();
            Cleaner cleaner = services.GetRequiredService<Cleaner>();
            cleaner.Start();

            Console.WriteLine();
            logger.LogInformation("Finished");

            if (options.Wait)
            {
                Console.ReadLine();
            }
        }

        private static IServiceProvider ConfigureServices(ProgramOptions options)
        {
            CleanerOptions cleanerOptions = new CleanerOptions
            {
                Simulate = options.Simulate,
            };

            return new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSerilog(SerilogLogging.Create(), dispose: true);
                })
                .AddSingleton(cleanerOptions)
                .AddTransient<Cleaner>()
                .BuildServiceProvider();
        }
    }
}