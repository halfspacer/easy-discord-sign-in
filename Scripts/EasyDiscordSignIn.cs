using Plugins.EasyDiscord.Scripts.Core;
using Plugins.EasyDiscord.Scripts.Objects;
using UnityEngine;
using UnityEngine.Events;

namespace Plugins.EasyDiscord.Scripts {
    public class EasyDiscordSignIn : MonoBehaviour {
        private static EasyDiscordSignIn _instance;
        public bool printDebugLogs;

        public static EasyDiscordSignIn Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<EasyDiscordSignIn>();
                    if (_instance == null) {
                        var obj = new GameObject {
                            name = nameof(EasyDiscordSignIn)
                        };
                        _instance = obj.AddComponent<EasyDiscordSignIn>();
                    }
                }

                return _instance;
            }
        }

        private void Awake() {
            DiscordRequest.printDebug = printDebugLogs;
        }

        public void SignIn() {
            onSignInBegin?.Invoke();
            DiscordRequest.Authorize(settings, signInResponse => {
                if (signInResponse.result == Result.Success) {
                    AccessToken = signInResponse.content;
                    DiscordRequest.GetCurrentUser(AccessToken.access_token, getUserResponse => {
                        if (getUserResponse.result == Result.Success) {
                            User = getUserResponse.content;
                            onSignInSuccess?.Invoke();
                        }
                        else {
                            onSignInFailed?.Invoke();
                        }
                        
                        onSignInEnd?.Invoke();
                    });
                }
                else {
                    onSignInFailed?.Invoke();
                }
            });
        }

        public void SaveAccessToken() => DiscordRequest.SaveAccessTokenToPlayerPrefs(AccessToken);
        public void ClearAccessToken() => DiscordRequest.ClearAccessTokenFromPlayerPrefs();

        #region Data

        public DiscordSettings settings;
        public AccessTokenResponse AccessToken { get; private set; }
        public UserResponse User { get; private set; }

        #endregion

        #region Events

        public UnityEvent onSignInSuccess;
        public UnityEvent onSignInFailed;
        public UnityEvent onSignInBegin;
        public UnityEvent onSignInEnd;
        public bool showEvents;

        #endregion
    }
}