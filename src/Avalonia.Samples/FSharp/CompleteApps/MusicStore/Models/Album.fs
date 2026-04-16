namespace MusicStore.Models

open System
open System.IO
open System.Net
open System.Net.Http
open System.Text.Json
open System.Threading.Tasks

open iTunesSearch.Library

/// Represents a music album with minimal metadata and a helper to search iTunes.
[<AllowNullLiteral>]
type Album(artist: string, title: string, coverUrl: string) =
    // Static search manager shared for all queries
    static let s_SearchManager = iTunesSearchManager ()
    // Shared HTTP client for downloading cover images
    static let s_httpClient =
        let handler = new HttpClientHandler()
        handler.AutomaticDecompression <- DecompressionMethods.GZip ||| DecompressionMethods.Deflate
        new HttpClient(handler, disposeHandler = true)

    static do
        // Configure HttpClient default headers once
        s_httpClient.Timeout <- TimeSpan.FromSeconds(15.0)

        try
            s_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MusicStore/1.0 (+https://github.com/AvaloniaUI/Avalonia.Samples)")
            s_httpClient.DefaultRequestHeaders.Accept.ParseAdd("image/webp,image/apng,image/*;q=0.8,*/*;q=0.5")
        with exn ->
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Failed to set HttpClient headers: {exn.ToString()}")
#endif

    static let mutable s_httpClientDisposed = false
    
    // The `CacheDir` is constructed from the current directory of the spawning
    // process. So while this directory will be in the expected location when using
    // the UI in VS or Rider to launch a debug instance, it may not be in the expected
    // location if launched directly from the terminal while still in the root solution directory, for example. You will also need to accomodate for this in any
    // tests you write.
    // This approach, while naive, is platform-agnostic at least.
    static member private CacheDir = Path.Join(Environment.CurrentDirectory, "Cache")

    /// Disposes the static HttpClient instance. In a real application, we would define the http client in a more central location
    /// and call this when the application is shutting down.
    static member DisposeHttpClient() =
        if not s_httpClientDisposed then
            s_httpClient.Dispose()
            s_httpClientDisposed <- true

    /// Sanitizes a string to be safe for file names
    static member private SanitizeFileName(input: string) =
        Path.GetInvalidFileNameChars()
        |> Array.fold (fun (acc: string) (c: char) -> acc.Replace(c, '_')) input

    /// Searches iTunes for albums matching the given term. Returns an empty sequence for null/whitespace.
    static member SearchAsync(searchTerm: string option) =
        task {
            let searchTerm =
                match searchTerm with
                | Some s -> s
                | None -> ""

            if String.IsNullOrWhiteSpace(searchTerm) then
                return Seq.empty<Album>
            else
                let! query = s_SearchManager.GetAlbumsAsync(searchTerm)

                let results =
                    query.Albums
                    |> Seq.map (fun x ->
                        let cover = x.ArtworkUrl100.Replace("100x100bb", "600x600bb")
                        new Album(x.ArtistName, x.CollectionName, cover))

                return results
        }

    static member private SaveToStreamAsync(data: Album, stream: Stream) =
        task { do! JsonSerializer.SerializeAsync(stream, data).ConfigureAwait(false) }

    static member LoadFromStream(stream: Stream) =
        task {
            let! album = JsonSerializer.DeserializeAsync<Album>(stream).ConfigureAwait(false)
            return album
        }

    static member LoadCachedAsync() =
        task {
            if not (Directory.Exists(Album.CacheDir)) then
                Directory.CreateDirectory(Album.CacheDir) |> ignore

            let results = ResizeArray<Album>()
            
            // Only consider JSON files produced by Album.SaveAsync()
            let files =
                Directory.EnumerateFiles(Album.CacheDir, "*.json", SearchOption.TopDirectoryOnly)

            let bag = System.Collections.Concurrent.ConcurrentBag<Album>()
            let opts = ParallelOptions(MaxDegreeOfParallelism = Environment.ProcessorCount)
            
            do! Parallel.ForEachAsync(
                    files,
                    opts,
                    fun file ct ->
                        task {
                            use fs = File.OpenRead(file)
                            let! album = Album.LoadFromStream(fs).ConfigureAwait(false)
                            bag.Add(album)
                        }
                        |> ValueTask
                )
            
            let albums = bag.ToArray()

            results.AddRange albums 

            return seq results
        }

    member val Artist: string = artist with get, set

    /// Album title
    member val Title: string = title with get, set

    /// <summary>
    /// Asynchronously loads the cover bitmap as a <see cref="System.IO.Stream"/>, using a cached file if available,
    /// or downloading from the cover URL otherwise.
    /// </summary>
    /// <returns>
    /// A <see cref="System.IO.Stream"/> containing the cover bitmap data. The caller is responsible for disposing the returned stream.
    /// </returns>
    /// <exception cref="System.IO.IOException">
    /// Thrown if there is an error accessing the cache file.
    /// </exception>
    /// <exception cref="System.Net.Http.HttpRequestException">
    /// Thrown if there is an error downloading the cover image from the URL.
    /// </exception>Cover (artwork) URL
    member val CoverUrl: string = coverUrl with get, set

    /// Cache file path for the cover bitmap
    member private this.CachePath =
        Path.Join(
            Album.CacheDir,
            $"{Album.SanitizeFileName(this.Artist)} - {Album.SanitizeFileName(this.Title)}"
        )

    /// Asynchronously loads the cover bitmap stream, using cache when available
    member this.LoadCoverBitmapAsync() =
        task {
            let bmpPath = this.CachePath + ".bmp"

            if File.Exists(bmpPath) then
                return File.OpenRead(bmpPath) :> Stream
            else
                // Download with a simple retry to handle transient CDN hiccups
                let rec download attempt =
                    task {
                        try
                            let! data = s_httpClient.GetByteArrayAsync(this.CoverUrl)
                            return new MemoryStream(data) :> Stream
                        with :? HttpRequestException when attempt < 2 ->
                            do! Task.Delay(200)
                            return! download (attempt + 1)
                    }

                return! download 0
        }

    member this.SaveAsync() =
        task {
            if not (Directory.Exists(Album.CacheDir)) then
                Directory.CreateDirectory(Album.CacheDir) |> ignore

            use fs = File.OpenWrite(this.CachePath + ".json")
            do! Album.SaveToStreamAsync(this, fs)
        }

    member this.SaveCoverBitmapStream() = File.OpenWrite(this.CachePath + ".bmp")
