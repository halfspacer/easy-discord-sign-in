using System;
using System.Linq;
using Plugins.EasyDiscord.Scripts.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Plugins.EasyDiscord.Scripts.Components {
    public class EDEnforceGuild : MonoBehaviour {
        [SerializeField] private string[] guildIds;
        public UnityEvent onUserInGuild;
        public UnityEvent onUserNotInGuild;

        public void EnforceGuild() {
            DiscordRequest.GetGuilds(EasyDiscordSignIn.Instance.AccessToken.access_token, (guilds) => {
                if (guilds.result != Result.Success) return;
                
                if (guilds.content.Any(guild => Array.Exists(guildIds, id => id == guild.id))) {
                    onUserInGuild?.Invoke();
                    return;
                }

                onUserNotInGuild?.Invoke();
            });
        }
    }
}