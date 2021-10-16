namespace StartMenuCleaner;

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

internal static partial class LoggerExtensions
{
    [LoggerMessage(501, LogLevel.Error, "Failed to clean {FilePath}. Aborting.",
        EventName = nameof(CleaningFailed))]
    public static partial void CleaningFailed(this ILogger logger, string filePath, Exception ex);

    [LoggerMessage(108, LogLevel.Information, "Finished Cleaning.",
        EventName = nameof(CleaningFinished))]
    public static partial void CleaningFinished(this ILogger logger);

    public static void CleaningItem(this ILogger logger, ProgramDirectoryItem item)
        => logger.CleaningItem(item.Reason, item.Path);

    [LoggerMessage(106, LogLevel.Information, "Cleaning.",
        EventName = nameof(CleaningStarted))]
    public static partial void CleaningStarted(this ILogger logger);

    [LoggerMessage(103, LogLevel.Information, "Debug logging is enabled",
        EventName = nameof(DebugEnabled))]
    public static partial void DebugEnabled(this ILogger logger);

    [LoggerMessage(301, LogLevel.Debug, "Deleted directory: \"{DirectoryName}\"",
        EventName = nameof(DirectoryDeleted))]
    public static partial void DirectoryDeleted(this ILogger logger, string directoryName);

    [LoggerMessage(302, LogLevel.Debug, "Deleted file: \"{FileName}\"",
        EventName = nameof(FileDeleted))]
    public static partial void FileDeleted(this ILogger logger, string fileName);

    [LoggerMessage(303, LogLevel.Debug, "Moved file: \"{FileName}\" to \"{NewLocation}\"",
        EventName = nameof(FileMoved))]
    public static partial void FileMoved(this ILogger logger, string fileName, string newLocation);

    [LoggerMessage(102, LogLevel.Information, "Finished",
        EventName = nameof(Finished))]
    public static partial void Finished(this ILogger logger);

    public static void FoundItemsToClean(this ILogger logger, IEnumerable<ProgramDirectoryItem> itemsToClean)
    {
        foreach (IGrouping<CleanReason, ProgramDirectoryItem> group in itemsToClean.GroupBy(x => x.Reason))
        {
            logger.FoundItemsToClean(group);
        }
    }

    public static void FoundItemsToClean(this ILogger logger, IGrouping<CleanReason, ProgramDirectoryItem> groupItems)
    {
        logger.FoundItemsToClean(groupItems.Count(), groupItems.Key);
        foreach (ProgramDirectoryItem item in groupItems)
        {
            logger.FoundItemsToCleanPath(item.Path);
        }
    }

    [LoggerMessage(105, LogLevel.Information, "Nothing to clean.",
        EventName = nameof(NothingToClean))]
    public static partial void NothingToClean(this ILogger logger);

    [LoggerMessage(104, LogLevel.Information, "Simulating. No changes will be made.",
        EventName = nameof(Simulating))]
    public static partial void Simulating(this ILogger logger);

    [LoggerMessage(101, LogLevel.Information, "Starting",
        EventName = nameof(Starting))]
    public static partial void Starting(this ILogger logger);

    [LoggerMessage(107, LogLevel.Information, "Cleaning {Reason} {Path}",
                                                    EventName = nameof(CleaningItem))]
    private static partial void CleaningItem(this ILogger logger, CleanReason reason, string path);

    [LoggerMessage(401, LogLevel.Trace, "Found {Count} {Type} items to clean:",
        EventName = nameof(FoundItemsToClean))]
    private static partial void FoundItemsToClean(this ILogger logger, int count, CleanReason type);

    [LoggerMessage(402, LogLevel.Trace, "\t{Path}",
        EventName = nameof(FoundItemsToCleanPath))]
    private static partial void FoundItemsToCleanPath(this ILogger logger, string path);
}
