using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using iTunesSearch.Library;

namespace Avalonia.MusicStore.Models
{
    public class Album
    {
        private static iTunesSearchManager s_SearchManager = new();
        private static HttpClient s_httpClient = new();

        public Album(string artist, string title, string coverUrl)
        {
            Artist = artist;
            Title = title;
            CoverUrl = coverUrl;
        }

        public string Artist { get; set; }
        public string Title { get; set; }
        public string CoverUrl { get; set; }
        private string CachePath => $"./Cache/{SanitizeFileName(Artist)} - {SanitizeFileName(Title)}";

        /// <summary>
        /// Searches in iTunes api for albums matching the given search term.
        /// </summary>
        public static async Task<IEnumerable<Album>> SearchAsync(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<Album>();
            }
            
            var query = await s_SearchManager.GetAlbumsAsync(searchTerm).ConfigureAwait(false);

            return query.Albums.Select(x =>
                new Album(x.ArtistName, x.CollectionName,
                    x.ArtworkUrl100.Replace("100x100bb", "600x600bb")));
        }

        /// <summary>
        /// Loads an album from the given stream.
        /// </summary>
        public static async Task<Album> LoadFromStream(Stream stream)
        {
            return (await JsonSerializer.DeserializeAsync<Album>(stream).ConfigureAwait(false))!;
        }

        /// <summary>
        /// Loads all cached albums from /Cache directory.
        /// </summary>
        public static async Task<IEnumerable<Album>> LoadCachedAsync()
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

                await using var fs = File.OpenRead(file);
                results.Add(await Album.LoadFromStream(fs).ConfigureAwait(false));
            }

            return results;
        }

        /// <summary>
        /// Loads the album cover bitmap from cache or api.
        /// </summary>
        public async Task<Stream> LoadCoverBitmapAsync()
        {
            if (File.Exists(CachePath + ".bmp"))
            {
                return File.OpenRead(CachePath + ".bmp");
            }
            else
            {
                var data = await s_httpClient.GetByteArrayAsync(CoverUrl);
                return new MemoryStream(data);
            }
        }

        /// <summary>
        /// Calls SaveToStreamAsync to save album data to the local cache and creates cache folder if it didn't exist.
        /// </summary>
        public async Task SaveAsync()
        {
            if (!Directory.Exists("./Cache"))
            {
                Directory.CreateDirectory("./Cache");
            }

            using (var fs = File.OpenWrite(CachePath + ".json"))
            {
                await SaveToStreamAsync(this, fs);
            }
        }

        /// <summary>
        /// Opens a stream for saving the album cover bitmap.
        /// </summary>
        public Stream SaveCoverBitmapStream()
        {
            return File.OpenWrite(CachePath + ".bmp");
        }

        /// <summary>
        /// Sanitizes invalid characters from the input and returns valid characters for a file name.
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

        /// <summary>
        /// Saves the album to the given stream as JSON.
        /// </summary>
        private static async Task SaveToStreamAsync(Album data, Stream stream)
        {
            await JsonSerializer.SerializeAsync(stream, data).ConfigureAwait(false);
        }
    }
}
