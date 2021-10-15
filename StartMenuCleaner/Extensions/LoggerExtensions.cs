namespace StartMenuCleaner;

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

internal static class LoggerExtensions
{
    private static readonly Action<ILogger, string, Exception?> cleaningFailed;
    private static readonly Action<ILogger, Exception?> cleaningFinished;
    private static readonly Action<ILogger, CleanReason, string, Exception?> cleaningItem;
    private static readonly Action<ILogger, Exception?> cleaningStarted;
    private static readonly Action<ILogger, Exception?> debugEnabled;
    private static readonly Action<ILogger, string, Exception?> directoryDeleted;
    private static readonly Action<ILogger, string, Exception?> fileDeleted;
    private static readonly Action<ILogger, string, string, Exception?> fileMoved;
    private static readonly Action<ILogger, Exception?> finished;
    private static readonly Action<ILogger, int, CleanReason, Exception?> foundItemsToClean;
    private static readonly Action<ILogger, string, Exception?> foundItemsToCleanPath;
    private static readonly Action<ILogger, Exception?> nothingToClean;
    private static readonly Action<ILogger, Exception?> simulating;
    private static readonly Action<ILogger, Exception?> starting;

    static LoggerExtensions()
    {
        cleaningFailed = LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(0, nameof(CleaningFailed)),
            "Failed to clean {Path}. Aborting.");

        cleaningFinished = LoggerMessage.Define(
            LogLevel.Information,
            new EventId(0, nameof(CleaningFinished)),
            "Finished Cleaning.");

        cleaningItem = LoggerMessage.Define<CleanReason, string>(
            LogLevel.Information,
            new EventId(0, nameof(CleaningItem)),
            "Cleaning {Reason} {Path}");

        cleaningStarted = LoggerMessage.Define(
            LogLevel.Information,
            new EventId(0, nameof(CleaningStarted)),
            "Cleaning.");

        debugEnabled = LoggerMessage.Define(
            LogLevel.Information,
            new EventId(0, nameof(DebugEnabled)),
            "Debug logging is enabled");

        directoryDeleted = LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(0, nameof(DirectoryDeleted)),
            "Deleted directory: \"{DirectoryName}\"");

        fileDeleted = LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(0, nameof(FileDeleted)),
            "Deleted file: \"{FileName}\"");

        fileMoved = LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            new EventId(0, nameof(FileMoved)),
            "Moved file: \"{FileName}\" to \"{NewLocation}\"");

        finished = LoggerMessage.Define(
            LogLevel.Information,
            new EventId(0, nameof(Finished)),
            "Finished");

        foundItemsToClean = LoggerMessage.Define<int, CleanReason>(
            LogLevel.Trace,
            new EventId(0, nameof(FoundItemsToClean)),
            "Found {Count} {Type} items to clean:");

        foundItemsToCleanPath = LoggerMessage.Define<string>(
            LogLevel.Trace,
            new EventId(0, nameof(FoundItemsToClean)),
            "\t{Path}");

        nothingToClean = LoggerMessage.Define(
            LogLevel.Information,
            new EventId(0, nameof(NothingToClean)),
            "Nothing to clean.");

        simulating = LoggerMessage.Define(
            LogLevel.Information,
            new EventId(0, nameof(Simulating)),
            "Simulating. No changes will be made.");

        starting = LoggerMessage.Define(
            LogLevel.Information,
            new EventId(0, nameof(Starting)),
            "Starting");
    }

    public static void CleaningFailed(this ILogger logger, string filePath, Exception ex)
        => cleaningFailed(logger, filePath, ex);

    public static void CleaningFinished(this ILogger logger) => cleaningFinished(logger, null);

    public static void CleaningItem(this ILogger logger, ProgramDirectoryItem item)
        => cleaningItem(logger, item.Reason, item.Path, null);

    public static void CleaningStarted(this ILogger logger) => cleaningStarted(logger, null);

    public static void DebugEnabled(this ILogger logger) => debugEnabled(logger, null);

    public static void DirectoryDeleted(this ILogger logger, string name) => directoryDeleted(logger, name, null);

    public static void FileDeleted(this ILogger logger, string name) => fileDeleted(logger, name, null);

    public static void FileMoved(this ILogger logger, string name, string newLocation)
        => fileMoved(logger, name, newLocation, null);

    public static void Finished(this ILogger logger) => finished(logger, null);

    public static void FoundItemsToClean(this ILogger logger, IEnumerable<ProgramDirectoryItem> itemsToClean)
    {
        foreach (IGrouping<CleanReason, ProgramDirectoryItem> group in itemsToClean.GroupBy(x => x.Reason))
        {
            logger.FoundItemsToClean(group);
        }
    }

    public static void FoundItemsToClean(this ILogger logger, IGrouping<CleanReason, ProgramDirectoryItem> groupItems)
    {
        foundItemsToClean(logger, groupItems.Count(), groupItems.Key, null);
        foreach (ProgramDirectoryItem item in groupItems)
        {
            foundItemsToCleanPath(logger, item.Path, null);
        }
    }

    public static void NothingToClean(this ILogger logger) => nothingToClean(logger, null);

    public static void Simulating(this ILogger logger) => simulating(logger, null);

    public static void Starting(this ILogger logger) => starting(logger, null);
}
