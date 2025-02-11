using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoundCloudDownloader.SoundCloud
{
    public class SoundCloudClient
    {
        private const string ClientId = "your_client_id";
        private readonly HttpClient _httpClient;

        public SoundCloudClient()
        {
            this._httpClient = new HttpClient();
        }


        private async Task<string[]> GetTracksFromPlaylist(string playlistUrl)
        {
            var tracks = new List<string>();

            var resolveUrl = $"https://api.soundcloud.com/resolve?url={playlistUrl}&client_id={ClientId}";
            var response = await _httpClient.GetAsync(resolveUrl);
            var contentStream = await response.Content.ReadAsStreamAsync();
            var playlist = await JsonSerializer.DeserializeAsync<Playlist>(contentStream);

            return tracks.ToArray();
        }
    }

    public class Playlist
    {
        public string Title { get; set; }
        public Track[] Tracks { get; set; }
    }

    public class Track
    {
        public string Title { get; set; }
        public string PermalinkUrl { get; set; }
    }
}
