using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;

namespace AllReady.Services.Twitter
{
    public class TwitterService : ITwitterService
    {
        private readonly IHttpClient _httpClient;
        private readonly TwitterAuthenticationSettings _twitterAuthenticationSettings;
        private readonly string _credentials;

        private const string TwitterAuthUrl = "https://api.twitter.com/oauth2/token";
        private const string TwitterUsersShowUrl = "https://api.twitter.com/1.1/users/show.json?";

        public TwitterService(IOptions<TwitterAuthenticationSettings> twitterAuthenticationSettings, IHttpClient httpClient)
        {
            _twitterAuthenticationSettings = twitterAuthenticationSettings.Value;
            _httpClient = httpClient;

            if (!AnyTwitterAuthenticationSettingsAreNotSet())
            {
                _credentials = GetBearerRequestCredentials(_twitterAuthenticationSettings.ConsumerKey,
                    _twitterAuthenticationSettings.ConsumerSecret);
            }
        }

        public async Task<TwitterUserInfo> GetTwitterAccount(string userId, string screenName)
        {
            if (string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(screenName))
            {
                return null;
            }

            var bearerToken = await RequestBearerToken();

            if (string.IsNullOrEmpty(bearerToken))
            {
                return null;
            }

            var requestUrl = new StringBuilder(TwitterUsersShowUrl);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                requestUrl.Append("user_id").Append("=").Append(userId).Append("&");
            }

            if (!string.IsNullOrWhiteSpace(screenName))
            {
                requestUrl.Append("screen_name").Append("=").Append(screenName);
            }

            if (requestUrl.ToString().EndsWith("&"))
                requestUrl.Remove(requestUrl.Length - 1, 1);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl.ToString());
            request.Headers.TryAddWithoutValidation("Authorization", string.Concat("Bearer ", bearerToken));

            try
            {
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var twitterResponse = JsonConvert.DeserializeObject<TwitterResponse>(content);

                    return new TwitterUserInfo { Name = twitterResponse.Name };
                }
            }
            catch
            {
                // todo - logging
            }

            return null;
        }

        private async Task<string> RequestBearerToken()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, TwitterAuthUrl);
            request.Headers.TryAddWithoutValidation("Authorization", _credentials);
            request.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            request.Headers.TryAddWithoutValidation("Host", "api.twitter.com");

            var values = new Dictionary<string, string> { { "grant_type", "client_credentials" } };
            var body = new FormUrlEncodedContent(values);
            request.Content = body;

            try
            {
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var twitterResponse = JsonConvert.DeserializeObject<TwitterResponse>(content);

                    if (twitterResponse?.TokenType == "bearer")
                    {
                        return twitterResponse?.AccessToken;
                    }
                }
            }
            catch
            {
                // todo - logging
            }

            return null;
        }

        private bool AnyTwitterAuthenticationSettingsAreNotSet()
        {
            return
                _twitterAuthenticationSettings.ConsumerKey == "[twitterconsumerkey]" ||
                _twitterAuthenticationSettings.ConsumerSecret == "[twitterconsumersecret]";
        }

        private string GetBearerRequestCredentials(string key, string secret)
        {
            var keyEncoded = UrlEncoder.Default.Encode(key);
            var secretEncoded = UrlEncoder.Default.Encode(secret);

            var keysecret = string.Concat(keyEncoded, ":", secretEncoded);

            var keySecretBytes = Encoding.UTF8.GetBytes(keysecret);

            var keySecretEncoded = Base64UrlTextEncoder.Encode(keySecretBytes);

            return string.Concat("Basic ", keySecretEncoded);
        }

        /// <summary>
        /// A generalised object to use for deserialisation of twitter responses
        /// </summary>
        private class TwitterResponse
        {
            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            
            public string Name { get; set; }
        }
    }
}
