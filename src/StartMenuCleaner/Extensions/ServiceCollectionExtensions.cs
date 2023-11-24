namespace StartMenuCleaner.Extensions;

using System.IO.Abstractions;

using Microsoft.Extensions.DependencyInjection;

using StartMenuCleaner.Utils;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the file shortcut handler.
    /// </summary>
    /// <typeparam name="TShortcutHandler">The type of the shortcut handler.</typeparam>
    /// <param name="services">The services.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection RegisterFileShortcutHandler<TShortcutHandler>(this IServiceCollection services)
        where TShortcutHandler : FileShortcutHandler
        => services.AddSingleton<FileShortcutHandler, TShortcutHandler>();

    /// <summary>
    /// Adds the file system to the service collection.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="fileSystem">The file system.</param>
    /// <returns><see cref="IServiceCollection" />.</returns>
    public static IServiceCollection UseFileSystem(
        this IServiceCollection services,
        IFileSystem? fileSystem = null)
        => services.AddSingleton(fileSystem ?? new FileSystem());
}
