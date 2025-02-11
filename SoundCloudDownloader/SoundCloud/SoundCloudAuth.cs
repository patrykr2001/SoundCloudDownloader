using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SoundCloudDownloader.SoundCloud
{
    public class SoundCloudAuth
    {
        public static string GetAuthorizationUrl(string clientId, string redirectUri, string codeVerifier)
        {
            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            return $"https://soundcloud.com/connect?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&code_challenge={codeChallenge}&code_challenge_method=S256";
        }

        private static string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.ASCII.GetBytes(codeVerifier);
                var hash = sha256.ComputeHash(bytes);
                return Base64UrlEncode(hash);
            }
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
