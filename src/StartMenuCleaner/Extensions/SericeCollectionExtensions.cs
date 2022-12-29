namespace StartMenuCleaner.Extensions;

using Microsoft.Extensions.DependencyInjection;
using StartMenuCleaner.Utils;
using System.IO.Abstractions;

internal static class SericeCollectionExtensions
{
    /// <summary>
    /// Registers the file shortcut handler.
    /// </summary>
    /// <typeparam name="TShortcutHandler">The type of the shortcut handler.</typeparam>
    /// <param name="services">The services.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection RegisterFileShortcutHandler<TShortcutHandler>(this IServiceCollection services)
        where TShortcutHandler : class, IFileShortcutHandler
        => services.AddSingleton<IFileShortcutHandler, TShortcutHandler>();

    /// <summary>
    /// Adds the file system to the service collection.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection UseFileSystem(this IServiceCollection services)
        => services.AddSingleton<IFileSystem>(new FileSystem());
}
