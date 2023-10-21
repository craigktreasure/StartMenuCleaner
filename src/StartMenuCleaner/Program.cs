namespace StartMenuCleaner;

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

using CommandLine;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using StartMenuCleaner.Cleaners.Directory;
using StartMenuCleaner.Cleaners.File;
using StartMenuCleaner.Extensions;
using StartMenuCleaner.Utils;

internal class Program
{
    public static void Run(ProgramOptions options, IFileSystem fileSystem)
    {
        IServiceProvider services = ConfigureServices(options, fileSystem);
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

    private static IServiceProvider ConfigureServices(ProgramOptions options, IFileSystem fileSystem)
    {
        CleanerOptions cleanerOptions = CleanerOptions.Load(options.Simulate);

        return new ServiceCollection()
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddDebug();
                loggingBuilder.AddSerilog(SerilogLogging.Create(), dispose: true);
            })
            .UseFileSystem(fileSystem)
            .RegisterFileShortcutHandler<CsWin32ShortcutHandler>()
            .AddSingleton<FileClassifier>()
            .AddSingleton<FileSystemOperationHandler>()
            .AddSingleton(cleanerOptions)
            .AddSingleton<Cleaner>()
            .AddSingleton<IManualConfigurationLoader, DotNetConfigManualConfigurationLoader>()
            .AddSingleton<IFileCleaner, ManualConfigurationFileCleaner>()
            .AddSingleton<IFileCleaner, BadShortcutFileCleaner>()
            .AddSingleton<IDirectoryCleaner, ManualConfigurationDirectoryCleaner>()
            .AddSingleton<IDirectoryCleaner, EmptyDirectoryCleaner>()
            .AddSingleton<IDirectoryCleaner, SingleAppDirectoryCleaner>()
            .AddSingleton<IDirectoryCleaner, FewAppsWithCruftDirectoryCleaner>()
            .BuildServiceProvider();
    }

    [ExcludeFromCodeCoverage]
    private static void Main(string[] args)
    {
        Console.Title = "Start Menu Cleaner";

        IFileSystem fileSystem = new FileSystem();
        Parser.Default.ParseArguments<ProgramOptions>(args).WithParsed(o => Run(o, fileSystem));
    }
}
