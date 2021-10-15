namespace StartMenuCleaner;

using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using StartMenuCleaner.Utils;
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

        logger.Starting();

        if (options.Debug)
        {
            SerilogLogging.SetMinLogLevel(Serilog.Events.LogEventLevel.Debug);
            logger.DebugEnabled();
        }

        Console.WriteLine();
        Cleaner cleaner = services.GetRequiredService<Cleaner>();
        cleaner.Start();

        Console.WriteLine();
        logger.Finished();

        if (options.Wait)
        {
            Console.ReadLine();
        }
    }

    private static IServiceProvider ConfigureServices(ProgramOptions options)
    {
        CleanerOptions cleanerOptions = new(StartMenuHelper.GetKnownStartMenuProgramsFolders())
        {
            Simulate = options.Simulate,
        };

        return new ServiceCollection()
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddDebug();
                loggingBuilder.AddSerilog(SerilogLogging.Create(), dispose: true);
            })
            .UseFileSystem()
            .UseDefaultFileShortcutHandler()
            .AddTransient<FileClassifier>()
            .AddTransient<CleanupRulesEngine>()
            .AddSingleton(cleanerOptions)
            .AddTransient<Cleaner>()
            .BuildServiceProvider();
    }
}
