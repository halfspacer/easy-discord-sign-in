using System;
using Newtonsoft.Json;

namespace Plugins.EasyDiscord.Scripts.Objects {
    [Serializable]
    public class InternalAccessTokenResponse {
        [JsonProperty("access_token", NullValueHandling = NullValueHandling.Ignore)]
        public string access_token;

        [JsonProperty("token_type", NullValueHandling = NullValueHandling.Ignore)]
        public string token_type;

        [JsonProperty("refresh_token", NullValueHandling = NullValueHandling.Ignore)]
        public string refresh_token;

        [JsonProperty("scope", NullValueHandling = NullValueHandling.Ignore)]
        public string scope;

        [JsonProperty("expires_in", NullValueHandling = NullValueHandling.Ignore)]
        public string expires_in;
    }

    [Serializable]
    public class AccessTokenResponse {
        [JsonProperty("access_token", NullValueHandling = NullValueHandling.Ignore)]
        public string access_token;

        [JsonProperty("token_type", NullValueHandling = NullValueHandling.Ignore)]
        public string token_type;

        [JsonProperty("refresh_token", NullValueHandling = NullValueHandling.Ignore)]
        public string refresh_token;

        [JsonProperty("scope", NullValueHandling = NullValueHandling.Ignore)]
        public string scope;

        [JsonProperty("expires_in", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime expires_in;
    }
}