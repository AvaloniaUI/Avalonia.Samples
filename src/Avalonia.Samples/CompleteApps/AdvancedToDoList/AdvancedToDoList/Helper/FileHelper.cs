using Avalonia.Platform.Storage;

namespace AdvancedToDoList.Helper;

public static class FileHelper
{
    public static FilePickerFileType JsonFileType { get; } = new FilePickerFileType("Json file")
    {
        Patterns = ["*.json"],
        AppleUniformTypeIdentifiers = ["public.json"],
        MimeTypes = ["application/json"],
    };
}