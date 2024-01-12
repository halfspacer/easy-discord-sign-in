using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plugins.EasyDiscord.Scripts.Objects {
    [Serializable]
    public class GuildResponse {
        [JsonProperty("afk_channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public object afk_channel_id;

        [JsonProperty("afk_timeout", NullValueHandling = NullValueHandling.Ignore)]
        public int? afk_timeout;

        [JsonProperty("application_id", NullValueHandling = NullValueHandling.Ignore)]
        public object application_id;

        [JsonProperty("banner", NullValueHandling = NullValueHandling.Ignore)]
        public string banner;

        [JsonProperty("default_message_notifications", NullValueHandling = NullValueHandling.Ignore)]
        public int? default_message_notifications;

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string description;

        [JsonProperty("discovery_splash", NullValueHandling = NullValueHandling.Ignore)]
        public object discovery_splash;

        [JsonProperty("emojis", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> emojis;

        [JsonProperty("explicit_content_filter", NullValueHandling = NullValueHandling.Ignore)]
        public int? explicit_content_filter;

        [JsonProperty("features", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> features;

        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string icon;

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string id;

        [JsonProperty("max_members", NullValueHandling = NullValueHandling.Ignore)]
        public int? max_members;

        [JsonProperty("max_presences", NullValueHandling = NullValueHandling.Ignore)]
        public int? max_presences;

        [JsonProperty("mfa_level", NullValueHandling = NullValueHandling.Ignore)]
        public int? mfa_level;

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string name;

        [JsonProperty("owner_id", NullValueHandling = NullValueHandling.Ignore)]
        public string owner_id;

        [JsonProperty("preferred_locale", NullValueHandling = NullValueHandling.Ignore)]
        public string preferred_locale;

        [JsonProperty("premium_subscription_count", NullValueHandling = NullValueHandling.Ignore)]
        public int? premium_subscription_count;

        [JsonProperty("premium_tier", NullValueHandling = NullValueHandling.Ignore)]
        public int? premium_tier;

        [JsonProperty("public_updates_channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public string public_updates_channel_id;

        [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
        public object region;

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> roles;

        [JsonProperty("rules_channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public string rules_channel_id;

        [JsonProperty("splash", NullValueHandling = NullValueHandling.Ignore)]
        public object splash;

        [JsonProperty("system_channel_flags", NullValueHandling = NullValueHandling.Ignore)]
        public int? system_channel_flags;

        [JsonProperty("system_channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public object system_channel_id;

        [JsonProperty("vanity_url_code", NullValueHandling = NullValueHandling.Ignore)]
        public string vanity_url_code;

        [JsonProperty("verification_level", NullValueHandling = NullValueHandling.Ignore)]
        public int? verification_level;

        [JsonProperty("widget_channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public object widget_channel_id;

        [JsonProperty("widget_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? widget_enabled;
    }
}