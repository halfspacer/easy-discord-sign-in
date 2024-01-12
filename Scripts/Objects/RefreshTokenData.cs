using System;
using Newtonsoft.Json;

namespace Plugins.EasyDiscord.Scripts.Objects {
    [Serializable]
    public class RefreshTokenData {
        [JsonProperty("client_id", NullValueHandling = NullValueHandling.Ignore)]
        public string client_id;

        [JsonProperty("client_secret", NullValueHandling = NullValueHandling.Ignore)]
        public string client_secret;

        [JsonProperty("grant_type", NullValueHandling = NullValueHandling.Ignore)]
        public string grant_type;

        [JsonProperty("refresh_token", NullValueHandling = NullValueHandling.Ignore)]
        public string refresh_token;
    }
}