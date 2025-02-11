using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace SoundCloudDownloader.SoundCloud
{
    public class SoundCloudClient
    {
        private const string ClientId = "";
        private const string ClientSecret = "";
        private const string RedirectUri = "http://localhost:5000/callback";
        private readonly HttpClient _httpClient;

        public SoundCloudClient()
        {
            this._httpClient = new HttpClient();
        }

        public async Task<string> TestOAuthAsync()
        {
            string response = string.Empty;

            var codeVerifier = PKCEHelper.GenerateCodeVerifier();
            var authorizationUrl = SoundCloudAuth.GetAuthorizationUrl(ClientId, RedirectUri, codeVerifier);

            Process.Start(new ProcessStartInfo
            {
                FileName = authorizationUrl,
                UseShellExecute = true
            });

            var code = await GetAuthorizationCodeAsync("");
            if (string.IsNullOrEmpty(code))
            {
                return "";
            }

            var soundCloudOAuth = new SoundCloudOAuth();
            var tokenResponse = await soundCloudOAuth.GetAccessTokenAsync(ClientId, ClientSecret, RedirectUri, codeVerifier, code);

            return tokenResponse ?? "";
        }

        private static async Task<string> GetAuthorizationCodeAsync(string redirectUri)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(redirectUri + "/");
            listener.Start();

            Console.WriteLine("Waiting for authorization response...");
            var context = await listener.GetContextAsync();
            var code = context.Request.QueryString["code"];

            var response = context.Response;
            var responseString = "<html><body>Authorization successful. You can close this window.</body></html>";
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length);
            responseOutput.Close();

            listener.Stop();
            return code;
        }

        private static string ExtractCodeFromUri(string uri)
        {
            var query = new Uri(uri).Query;
            var queryParams = HttpUtility.ParseQueryString(query);
            return queryParams["code"];
        }

        public async Task<(long TotalBytes, HttpContent Content)> StartTrackDownloadAsync(string trackUrl)
        {
            var resolveUrl = $"{trackUrl}?client_id={ClientId}";

            var response = await _httpClient.GetAsync(resolveUrl);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var canReportProgress = totalBytes != -1;
            if (!canReportProgress)
            {
                throw new Exception($"Cannot start downloading track {trackUrl}. Content length is undefined.");
            }

            return (totalBytes, response.Content);
        }

        public async Task<Track[]> GetTracksFromPlaylistAsync(string playlistUrl)
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
