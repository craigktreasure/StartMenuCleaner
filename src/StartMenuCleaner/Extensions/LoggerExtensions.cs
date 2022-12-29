namespace StartMenuCleaner;

using Microsoft.Extensions.Logging;
using StartMenuCleaner.Cleaners;

internal static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to clean {FilePath}. Aborting.", EventName = nameof(CleaningFailed))]
    public static partial void CleaningFailed(this ILogger logger, string filePath, Exception ex);

    [LoggerMessage(Level = LogLevel.Information, Message = "Finished Cleaning.", EventName = nameof(CleaningFinished))]
    public static partial void CleaningFinished(this ILogger logger);

    public static void CleaningItem(this ILogger logger, ItemToClean item)
        => logger.CleaningItem(item.CleanerType, item.Path);

    [LoggerMessage(Level = LogLevel.Information, Message = "Cleaning.", EventName = nameof(CleaningStarted))]
    public static partial void CleaningStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Debug logging is enabled", EventName = nameof(DebugEnabled))]
    public static partial void DebugEnabled(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Deleted directory: \"{DirectoryName}\"", EventName = nameof(DirectoryDeleted))]
    public static partial void DirectoryDeleted(this ILogger logger, string directoryName);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Deleted file: \"{FileName}\"", EventName = nameof(FileDeleted))]
    public static partial void FileDeleted(this ILogger logger, string fileName);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Moved file: \"{FileName}\" to \"{NewLocation}\"", EventName = nameof(FileMoved))]
    public static partial void FileMoved(this ILogger logger, string fileName, string newLocation);

    [LoggerMessage(Level = LogLevel.Information, Message = "Finished", EventName = nameof(Finished))]
    public static partial void Finished(this ILogger logger);

    public static void FoundItemsToClean(this ILogger logger, IEnumerable<ItemToClean> itemsToClean)
    {
        foreach (IGrouping<CleanerType, ItemToClean> group in itemsToClean.GroupBy(x => x.CleanerType))
        {
            logger.FoundItemsToClean(group);
        }
    }

    public static void FoundItemsToClean(this ILogger logger, IGrouping<CleanerType, ItemToClean> groupItems)
    {
        logger.FoundItemsToClean(groupItems.Count(), groupItems.Key);
        foreach (ItemToClean item in groupItems)
        {
            logger.FoundItemsToCleanPath(item.Path);
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Nothing to clean.", EventName = nameof(NothingToClean))]
    public static partial void NothingToClean(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "This operating system is not supported.", EventName = nameof(OperatingSystemUnsupported))]
    public static partial void OperatingSystemUnsupported(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Simulating. No changes will be made.", EventName = nameof(Simulating))]
    public static partial void Simulating(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Starting", EventName = nameof(Starting))]
    public static partial void Starting(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Cleaning {CleanerType} {Path}", EventName = nameof(CleaningItem))]
    private static partial void CleaningItem(this ILogger logger, CleanerType cleanerType, string path);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Found {Count} {Type} items to clean:", EventName = nameof(FoundItemsToClean))]
    private static partial void FoundItemsToClean(this ILogger logger, int count, CleanerType type);

    [LoggerMessage(Level = LogLevel.Trace, Message = "\t{Path}", EventName = nameof(FoundItemsToCleanPath))]
    private static partial void FoundItemsToCleanPath(this ILogger logger, string path);
}
