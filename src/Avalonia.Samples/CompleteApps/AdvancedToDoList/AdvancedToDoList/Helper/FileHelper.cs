using Avalonia.Platform.Storage;

namespace AdvancedToDoList.Helper;

/// <summary>
/// An internal helper for working with File-I/O.
/// </summary>
internal static class FileHelper
{
    /// <summary>
    /// Gets the <see cref="FilePickerFileType"/> that represents a JSON file. 
    /// </summary>
    /// <seealso href="https://developer.apple.com/documentation/uniformtypeidentifiers"/>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/MIME_types/Common_types"/>
    internal static FilePickerFileType JsonFileType { get; } = new FilePickerFileType("Json file")
    {
        Patterns = ["*.json"],
        AppleUniformTypeIdentifiers = ["public.json"],
        MimeTypes = ["application/json"],
    };
}