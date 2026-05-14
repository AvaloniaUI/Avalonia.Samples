namespace MusicStore.Services

open System
open System.IO
open System.Net
open System.Net.Http
open System.Text.Json
open System.Threading.Tasks
open System.Collections.Concurrent
open MusicStore.Models

/// Private DTOs for deserializing the iTunes Search API response
[<CLIMutable>]
type private ItunesAlbum = {
    ArtistName: string
    CollectionName: string
    ArtworkUrl100: string
}

[<CLIMutable>]
type private ItunesSearchResult = {
    Results: ItunesAlbum[]
}

/// Provides album search, cover-art loading, and local cache persistence
/// by calling the Apple iTunes Web API.
type AlbumService() =
    static let s_jsonOptions = JsonSerializerOptions(PropertyNameCaseInsensitive = true)

    // Shared HTTP client for downloading cover images
    static let s_httpClient =
        let handler = new HttpClientHandler()
        handler.AutomaticDecompression <- DecompressionMethods.GZip ||| DecompressionMethods.Deflate
        new HttpClient(handler, disposeHandler = true)

    static do
        s_httpClient.Timeout <- TimeSpan.FromSeconds(15.0)

        try
            s_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MusicStore/1.0 (+https://github.com/AvaloniaUI/Avalonia.Samples)")
            s_httpClient.DefaultRequestHeaders.Accept.ParseAdd("image/webp,image/apng,image/*;q=0.8,*/*;q=0.5")
        with exn ->
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Failed to set HttpClient headers: {exn.ToString()}")
#endif
            ()

    // The `CacheDir` is constructed from the current directory of the spawning process.
    static member private CacheDir = Path.Join(Environment.CurrentDirectory, "Cache")

    /// Replaces characters that are invalid in file names with underscores.
    static member private SanitizeFileName(input: string) =
        Path.GetInvalidFileNameChars()
        |> Array.fold (fun (acc: string) (c: char) -> acc.Replace(c, '_')) input

    member private _.CachePath(album: Album) =
        Path.Join(
            AlbumService.CacheDir,
            $"{AlbumService.SanitizeFileName(album.Artist)} - {AlbumService.SanitizeFileName(album.Title)}"
        )

    /// Searches iTunes for albums matching the given term. Returns an empty sequence for null/whitespace.
    member _.SearchAsync(searchTerm: string option) =
        task {
            let term =
                match searchTerm with
                | Some s -> s
                | None -> ""

            if String.IsNullOrWhiteSpace(term) then
                return Seq.empty<Album>
            else
                let url = $"https://itunes.apple.com/search?term={Uri.EscapeDataString(term)}&entity=album&limit=25"
                let! json = s_httpClient.GetStringAsync(url)
                let result = JsonSerializer.Deserialize<ItunesSearchResult>(json, s_jsonOptions)

                let results =
                    result.Results
                    |> Seq.map (fun x ->
                        let cover = x.ArtworkUrl100.Replace("100x100bb", "600x600bb")
                        new Album(x.ArtistName, x.CollectionName, cover))

                return results
        }

    /// Loads the cover bitmap for an album from cache or the iTunes API.
    member this.LoadCoverBitmapAsync(album: Album) =
        // Download with a simple retry to handle transient CDN hiccups.
        // Must be defined outside the outer task{} to avoid FS3511 (let rec in resumable code).
        let rec download attempt =
            task {
                try
                    let! data = s_httpClient.GetByteArrayAsync(album.CoverUrl)
                    return new MemoryStream(data) :> Stream
                with :? HttpRequestException when attempt < 2 ->
                    do! Task.Delay(200)
                    return! download (attempt + 1)
            }

        task {
            let bmpPath = this.CachePath(album) + ".bmp"

            if File.Exists(bmpPath) then
                return File.OpenRead(bmpPath) :> Stream
            else
                return! download 0
        }

    /// Saves the album metadata to the local cache as a JSON file.
    member this.SaveAsync(album: Album) =
        task {
            if not (Directory.Exists(AlbumService.CacheDir)) then
                Directory.CreateDirectory(AlbumService.CacheDir) |> ignore

            use fs = File.OpenWrite(this.CachePath(album) + ".json")
            do! JsonSerializer.SerializeAsync(fs, album).ConfigureAwait(false)
        }

    /// Opens a write stream for saving the album cover bitmap to cache.
    member this.SaveCoverBitmapStream(album: Album) =
        File.OpenWrite(this.CachePath(album) + ".bmp")

    /// Loads all cached albums from the Cache directory.
    member _.LoadCachedAsync() =
        task {
            if not (Directory.Exists(AlbumService.CacheDir)) then
                Directory.CreateDirectory(AlbumService.CacheDir) |> ignore

            let files =
                Directory.EnumerateFiles(AlbumService.CacheDir, "*.json", SearchOption.TopDirectoryOnly)

            let bag = ConcurrentBag<Album>()
            let opts = ParallelOptions(MaxDegreeOfParallelism = Environment.ProcessorCount)

            do! Parallel.ForEachAsync(
                    files,
                    opts,
                    fun file _ct ->
                        task {
                            use fs = File.OpenRead(file)
                            let! album = JsonSerializer.DeserializeAsync<Album>(fs).ConfigureAwait(false)
                            bag.Add(album)
                        }
                        |> ValueTask
                )

            return seq (bag.ToArray())
        }
