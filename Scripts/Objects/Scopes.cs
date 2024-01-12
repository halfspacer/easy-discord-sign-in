using System;

namespace Plugins.EasyDiscord.Scripts.Objects {
    //Discord OAuth2 Scopes
    [Flags]
    [Serializable]
    public enum Scopes {
        identify = 1 << 0,
        email = 1 << 1,
        connections = 1 << 2,
        guilds = 1 << 3,
        guilds_join = 1 << 4,
        guilds_members_read = 1 << 5,
        gdm_join = 1 << 6,
        messages_read = 1 << 7
    }
}