namespace MusicStore.Models

/// Represents a music album with its core metadata.
[<AllowNullLiteral>]
type Album(artist: string, title: string, coverUrl: string) =
    member val Artist: string = artist with get, set
    member val Title: string = title with get, set
    member val CoverUrl: string = coverUrl with get, set
