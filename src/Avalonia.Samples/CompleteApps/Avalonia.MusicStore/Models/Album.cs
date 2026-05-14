namespace Avalonia.MusicStore.Models
{
    // We use a record for Album since it helps to encapsulate data. 
    // See: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record 
    public record Album(string Artist, string Title, string CoverUrl);
}
