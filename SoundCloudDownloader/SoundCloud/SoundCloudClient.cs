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

        private async Task<Track[]> GetTracksFromPlaylistAsync(string playlistUrl)
        {
            var resolveUrl = $"https://api.soundcloud.com/resolve?url={playlistUrl}&client_id={ClientId}";

            var response = await _httpClient.GetAsync(resolveUrl);
            var contentStream = await response.Content.ReadAsStreamAsync();
            var playlist = await JsonSerializer.DeserializeAsync<Playlist>(contentStream);

            if (playlist != null)
            {
                return playlist.Tracks;
            }
            else
            {
                throw new Exception($"Playlist {playlistUrl} not found.");
            }
        }

        private async Task<byte[]> DownloadTrackAsync(string trackUrl)
        {
            var resolveUrl = $"{trackUrl}?client_id={ClientId}";

            var response = await _httpClient.GetAsync(resolveUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
