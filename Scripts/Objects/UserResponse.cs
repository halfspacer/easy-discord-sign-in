using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Plugins.EasyDiscord.Scripts.Objects {
    [Serializable]
    public class InternalUserResponse {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string id;

        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string username;

        [JsonProperty("discriminator", NullValueHandling = NullValueHandling.Ignore)]
        public string discriminator;

        [JsonProperty("avatar", NullValueHandling = NullValueHandling.Ignore)]
        public string avatar;

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string email;

        [JsonProperty("banner", NullValueHandling = NullValueHandling.Ignore)]
        public string banner;

        [JsonProperty("accent_color", NullValueHandling = NullValueHandling.Ignore)]
        public int? accent_color;

        [JsonProperty("flags", NullValueHandling = NullValueHandling.Ignore)]
        public int? flags;

        [JsonProperty("premium_type", NullValueHandling = NullValueHandling.Ignore)]
        public int? premium_type;

        [JsonProperty("public_flags", NullValueHandling = NullValueHandling.Ignore)]
        public int? public_flags;

        [JsonProperty("verified", NullValueHandling = NullValueHandling.Ignore)]
        public bool? verified;
    }

    [Serializable]
    public class UserResponse {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string id;

        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string username;

        [JsonProperty("discriminator", NullValueHandling = NullValueHandling.Ignore)]
        public string discriminator;

        [JsonProperty("avatar", NullValueHandling = NullValueHandling.Ignore)]
        public Texture2D avatar;

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string email;

        [JsonProperty("banner", NullValueHandling = NullValueHandling.Ignore)]
        public string banner;

        [JsonProperty("accent_color", NullValueHandling = NullValueHandling.Ignore)]
        public int? accent_color;

        [JsonProperty("flags", NullValueHandling = NullValueHandling.Ignore)]
        public int? flags;

        [JsonProperty("premium_type", NullValueHandling = NullValueHandling.Ignore)]
        public int? premium_type;

        [JsonProperty("public_flags", NullValueHandling = NullValueHandling.Ignore)]
        public int? public_flags;

        [JsonProperty("verified", NullValueHandling = NullValueHandling.Ignore)]
        public bool? verified;
    }
}