
using Avalonia;
using Avalonia.Controls;

namespace AdvancedToDoList.Helper;

/// <summary>
/// Internal helper class for accessing application-level resources.
/// Provides a centralized method for retrieving resources from Application.Current.
/// Used to access styles, templates, colors, and other application-wide resources.
/// </summary>
internal static class ResourcesHelper
{
    /// <summary>
    /// Looks up a resource from the application's resource dictionary.
    /// Safely retrieves resources with type checking and null handling.
    /// Returns the default value if the resource is not found or the type doesn't match.
    /// </summary>
    /// <param name="resourceKey">The key of the resource to lookup</param>
    /// <typeparam name="T">The expected type of the resource</typeparam>
    /// <returns>The found resource if present and type matches, otherwise default value</returns>
    internal static T? GetAppResource<T>(object resourceKey)
    {
        // Try to find the resource in Application.Current's resource dictionary
        var found = Application.Current!.TryFindResource(resourceKey, out var resource);
        if (found && resource is T value)
        {
            return value;
        }
        return default;
    }
}