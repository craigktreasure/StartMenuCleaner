namespace StartMenuCleaner;

using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using StartMenuCleaner.Cleaners.Directory;
using StartMenuCleaner.Cleaners.File;
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

        if (!OperatingSystem.IsWindows())
        {
            logger.OperatingSystemUnsupported();
            return;
        }

        logger.Starting();

        if (options.Debug)
        {
            SerilogLogging.SetMinLogLevel(Serilog.Events.LogEventLevel.Verbose);
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
            .RegisterFileShortcutHandler<CsWin32ShortcutHandler>()
            .AddSingleton<FileClassifier>()
            .AddSingleton<FileSystemOperationHandler>()
            .AddSingleton<CleanupRulesEngine>()
            .AddSingleton(cleanerOptions)
            .AddSingleton<Cleaner>()
            .AddSingleton<FileCleaner>()
            .AddSingleton<IFileCleaner, BadShortcutFileCleaner>()
            .AddSingleton<DirectoryCleaner>()
            .AddSingleton<IDirectoryCleaner, EmptyDirectoryCleaner>()
            .BuildServiceProvider();
    }
}
