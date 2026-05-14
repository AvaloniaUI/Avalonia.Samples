using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.MusicStore.Models;

namespace Avalonia.MusicStore.Services
{
    /// <summary>
    /// Provides album search, cover-art loading, and local cache persistence
    /// by calling the Apple iTunes Web API.
    /// </summary>
    public class AlbumService
    {
        private static readonly HttpClient s_httpClient = new();
        private static readonly JsonSerializerOptions s_jsonOptions = new() { PropertyNameCaseInsensitive = true };

        // Private DTOs for deserializing the iTunes Search API response
        private record ItunesAlbum(string ArtistName, string CollectionName, string ArtworkUrl100);
        private record ItunesSearchResult(ItunesAlbum[] Results);

        private static string CachePath(Album album) =>
            $"./Cache/{SanitizeFileName(album.Artist)} - {SanitizeFileName(album.Title)}";

        /// <summary>
        /// Searches the iTunes API for albums matching the given search term.
        /// </summary>
        public async Task<IEnumerable<Album>> SearchAsync(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<Album>();
            }

            var url = $"https://itunes.apple.com/search?term={Uri.EscapeDataString(searchTerm)}&entity=album&limit=25";
            var json = await s_httpClient.GetStringAsync(url).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<ItunesSearchResult>(json, s_jsonOptions);

            return result?.Results.Select(x =>
                new Album(x.ArtistName, x.CollectionName,
                    x.ArtworkUrl100.Replace("100x100bb", "600x600bb")))
                ?? Enumerable.Empty<Album>();
        }

        /// <summary>
        /// Loads the cover bitmap for an album from cache or the iTunes API.
        /// </summary>
        public async Task<Stream> LoadCoverBitmapAsync(Album album)
        {
            var cachePath = CachePath(album);
            if (File.Exists(cachePath + ".bmp"))
            {
                return File.Open(cachePath + ".bmp", FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else
            {
                var data = await s_httpClient.GetByteArrayAsync(album.CoverUrl);
                return new MemoryStream(data);
            }
        }

        /// <summary>
        /// Saves the album metadata to the local cache as a JSON file.
        /// </summary>
        public async Task SaveAsync(Album album)
        {
            if (!Directory.Exists("./Cache"))
            {
                Directory.CreateDirectory("./Cache");
            }

            using var fs = File.OpenWrite(CachePath(album) + ".json");
            await SaveToStreamAsync(album, fs);
        }

        /// <summary>
        /// Opens a write stream for saving the album cover bitmap to cache.
        /// </summary>
        public Stream SaveCoverBitmapStream(Album album)
        {
            return File.OpenWrite(CachePath(album) + ".bmp");
        }

        /// <summary>
        /// Loads all cached albums from the /Cache directory.
        /// </summary>
        public async Task<IEnumerable<Album>> LoadCachedAsync()
        {
            if (!Directory.Exists("./Cache"))
            {
                Directory.CreateDirectory("./Cache");
            }

            var results = new List<Album>();

            foreach (var file in Directory.EnumerateFiles("./Cache"))
            {
                if ((new DirectoryInfo(file).Extension) != ".json")
                    continue;

                await using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                results.Add(await LoadFromStreamAsync(fs).ConfigureAwait(false));
            }

            return results;
        }

        private static async Task<Album> LoadFromStreamAsync(Stream stream)
        {
            return (await JsonSerializer.DeserializeAsync<Album>(stream).ConfigureAwait(false))!;
        }

        private static async Task SaveToStreamAsync(Album data, Stream stream)
        {
            await JsonSerializer.SerializeAsync(stream, data).ConfigureAwait(false);
        }

        /// <summary>
        /// Replaces characters that are invalid in file names with underscores.
        /// Example: AC/DC -> AC_DC
        /// </summary>
        private static string SanitizeFileName(string input)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }

            return input;
        }
    }
}
