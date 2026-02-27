using Avalonia.Platform.Storage;

namespace AdvancedToDoList.Helper;

/// <summary>
/// Internal helper class for file input/output operations.
/// Provides platform-specific file type definitions for cross-platform compatibility.
/// Used primarily for data import/export functionality in the application.
/// </summary>
internal static class FileHelper
{
    /// <summary>
    /// Gets the FilePickerFileType configuration for JSON files.
    /// Provides cross-platform file type definitions for Windows, macOS, Linux, and web platforms.
    /// Ensures proper file association and MIME type handling across different operating systems.
    /// </summary>
    /// <seealso href="https://developer.apple.com/documentation/uniformtypeidentifiers"/>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/MIME_types/Common_types"/>
    internal static FilePickerFileType JsonFileType { get; } = new FilePickerFileType("Json file")
    {
        // Windows file pattern for file dialogs
        Patterns = ["*.json"],
        // macOS Uniform Type Identifier for JSON files
        AppleUniformTypeIdentifiers = ["public.json"],
        // Web and Linux MIME type for JSON files
        MimeTypes = ["application/json"],
    };
}