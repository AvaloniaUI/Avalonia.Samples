
using Avalonia;
using Avalonia.Controls;

namespace AdvancedToDoList.Helper;

/// <summary>
/// This is an interal helper class to work with App-resources.
/// </summary>
internal static class ResourcesHelper
{
    /// <summary>
    /// This method will look up all App-resources and return the found value or the default if nothing was found.
    /// </summary>
    /// <param name="resourceKey">The resource key to lookup.</param>
    /// <typeparam name="T">The expected return type.</typeparam>
    /// <returns>the found value if present, otherwise its default value.</returns>
    internal static T? GetAppResource<T>(object resourceKey)
    {
        var found = Application.Current!.TryFindResource(resourceKey, out var resource);
        if (found && resource is T value)
        {
            return value;
        }
        return default;
    }
}