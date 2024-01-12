using Plugins.EasyDiscord.Scripts.Core;
using Plugins.EasyDiscord.Scripts.Objects;
using UnityEngine;
using UnityEngine.Events;

namespace Plugins.EasyDiscord.Scripts.Components {
    /// <summary>
    /// A component that can be added to a GameObject to set the user's avatar without writing code.
    /// </summary>
    public class EDAvatarDisplay : MonoBehaviour {
        public UnityEvent<Texture2D> onAvailableTexture;
        [Tooltip("This will create a Sprite from the Texture2D, this is a bit more expensive than just using the Texture2D.")]
        public UnityEvent<Sprite> onAvailableSprite;

        private void OnEnable() => DiscordRequest.OnUserResponseUpdated += OnUserResponseUpdated;
        private void OnDisable() => DiscordRequest.OnUserResponseUpdated -= OnUserResponseUpdated;

        private void OnUserResponseUpdated(UserResponse userResponse) {
            if (userResponse.avatar == null) {
                return;
            }

            onAvailableTexture?.Invoke(userResponse.avatar);
            onAvailableSprite?.Invoke(Sprite.Create(userResponse.avatar,
                new Rect(0, 0, userResponse.avatar.width, userResponse.avatar.height), new Vector2(0.5f, 0.5f)));
        }
    }
}