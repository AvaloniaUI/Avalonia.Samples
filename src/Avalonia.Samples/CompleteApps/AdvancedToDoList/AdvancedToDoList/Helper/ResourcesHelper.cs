
using Avalonia;
using Avalonia.Controls;

namespace AdvancedToDoList.Helper;

internal static class ResourcesHelper
{
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