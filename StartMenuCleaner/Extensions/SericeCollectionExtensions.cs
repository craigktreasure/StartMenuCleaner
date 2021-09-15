namespace StartMenuCleaner;

using Microsoft.Extensions.DependencyInjection;
using StartMenuCleaner.Utils;
using System.IO.Abstractions;

internal static class SericeCollectionExtensions
{
    /// <summary>
    /// Adds the default file shortcut handler to the service collection.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection UseDefaultFileShortcutHandler(this IServiceCollection services)
                => services.AddSingleton<IFileShortcutHandler>(new DefaultFileShortcutHandler());

    /// <summary>
    /// Adds the file system to the service collection.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection UseFileSystem(this IServiceCollection services)
        => services.AddSingleton<IFileSystem>(new FileSystem());
}
