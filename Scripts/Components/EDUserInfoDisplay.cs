using Plugins.EasyDiscord.Scripts.Core;
using Plugins.EasyDiscord.Scripts.Objects;
using UnityEngine;
using UnityEngine.Events;

namespace Plugins.EasyDiscord.Scripts.Components {
    /// <summary>
    /// A component that can be added to a GameObject to set the User Info values without writing code.
    /// </summary>
    public class EDUserInfoDisplay : MonoBehaviour {
        public enum UserInfo {
            Username,
            Discriminator,
            ID,
            Email
        }

        public UserInfo info;
        public UnityEvent<string> onAvailable;

        private void OnEnable() => DiscordRequest.OnUserResponseUpdated += OnUserResponseUpdated;
        private void OnDisable() => DiscordRequest.OnUserResponseUpdated -= OnUserResponseUpdated;

        private void OnUserResponseUpdated(UserResponse userResponse) {
            if (EasyDiscordSignIn.Instance.User == null) {
                return;
            }

            switch (info) {
                case UserInfo.Username:
                    onAvailable?.Invoke(userResponse.username);
                    break;
                case UserInfo.Discriminator:
                    onAvailable?.Invoke(userResponse.discriminator);
                    break;
                case UserInfo.ID:
                    onAvailable?.Invoke(userResponse.id);
                    break;
                case UserInfo.Email:
                    onAvailable?.Invoke(userResponse.email);
                    break;
            }
        }
    }
}