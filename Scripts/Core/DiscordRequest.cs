using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Plugins.EasyDiscord.Scripts.Objects;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Plugins.EasyDiscord.Scripts.Core {
    public static class DiscordRequest {
        private const string HttpsDiscordComApiV10UsersMe = "https://discord.com/api/v10/users/@me";
        private const string HttpsDiscordComApiV10Oauth2Token = "https://discord.com/api/v10/oauth2/token";
        private const string HttpsDiscordComApiV10UsersMeGuilds = "https://discord.com/api/v10/users/@me/guilds";
        private const string HttpsCdnDiscordAppComAvatarsPNG = "https://cdn.discordapp.com/avatars/{0}/{1}.png";
        private const string HttpsCdnDiscordAppComEmbedAvatarsPNG = "https://cdn.discordapp.com/embed/avatars/{0}.png";
        private const string HttpsDiscordComApiV10Authorize = "https://discord.com/api/v10/oauth2/authorize?client_id={0}&redirect_uri={1}&response_type=code&scope={2}";
        private const string EasyDiscordColoredName = "<color=#7289DA>EasyDiscord:</color>";
        public static bool printDebug;
        private static (string, Texture2D) _avatar;
        public static event Action<UserResponse> OnUserResponseUpdated;

        private static void GetAccessToken(string code, DiscordSettings settings,
            Action<Response<InternalAccessTokenResponse>> onComplete) {
            var scopesString = GetScopesFormatted(settings);
            
            var body = new Dictionary<string, string> {
                { "client_id", settings.clientId },
                { "client_secret", settings.clientSecret },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", settings.redirectUri }
            };
            Post(HttpsDiscordComApiV10Oauth2Token, body, null, onComplete);
        }

        private static string GetScopesFormatted(DiscordSettings settings) {
            var scopesString = Enum.GetValues(typeof(Scopes)).Cast<object>()
                .Where(scope => settings.scopes.HasFlag((Scopes)scope)).Select(scope => scope.ToString().Replace("_", "."))
                .Aggregate((current, next) => current + "%20" + next);
            return scopesString;
        }

        public static void RefreshAccessToken(string refreshToken, DiscordSettings settings,
            Action<Response<AccessTokenResponse>> onComplete) {
            var body = new Dictionary<string, string> {
                { "client_id", settings.clientId },
                { "client_secret", settings.clientSecret },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "redirect_uri", settings.redirectUri }
            };
            Post<InternalAccessTokenResponse>(HttpsDiscordComApiV10Oauth2Token, body, null, response => {
                if (response.result == Result.Success && response.content.expires_in != null) {
                    var accessTokenResponse = new AccessTokenResponse {
                        access_token = response.content.access_token,
                        token_type = response.content.token_type,
                        refresh_token = response.content.refresh_token,
                        scope = response.content.scope,
                        expires_in = DateTime.Now.AddSeconds(int.Parse(response.content.expires_in))
                    };

                    onComplete?.Invoke(new Response<AccessTokenResponse> {
                        result = Result.Success,
                        content = accessTokenResponse,
                        message = response.message
                    });
                }
                else {
                    onComplete?.Invoke(new Response<AccessTokenResponse> {
                        result = Result.Failure,
                        content = default,
                        message = response.message
                    });
                }
            });
        }

        /// <summary>
        /// Gets the current user's profile information from the Discord API.
        /// Discord returns the user's avatar as a hash, but this method does a second request to get the actual image.
        /// </summary>
        /// <param name="accessToken">A valid Access Token</param>
        /// <param name="onComplete">
        /// A Response Object. Contains the UserResponse and well as Results, and Message containing any
        /// error information.
        /// </param>
        public static void GetCurrentUser(string accessToken, Action<Response<UserResponse>> onComplete) {
            var headers = new Dictionary<string, string> {
                { "Authorization", $"Bearer {accessToken}" }
            };
            Get<InternalUserResponse>(HttpsDiscordComApiV10UsersMe, headers, response => {
                if (response.result == Result.Success) {
                    GetAvatar(accessToken, response.content, (result, texture2D) => {
                        if (result == Result.Success) {
                            var userReponse = new UserResponse {
                                id = response.content.id,
                                username = response.content.username,
                                discriminator = response.content.discriminator,
                                avatar = texture2D,
                                verified = response.content.verified,
                                email = response.content.email,
                                flags = response.content.flags,
                                banner = response.content.banner,
                                accent_color = response.content.accent_color,
                                premium_type = response.content.premium_type,
                                public_flags = response.content.public_flags
                            };

                            onComplete?.Invoke(new Response<UserResponse> {
                                result = Result.Success,
                                content = userReponse,
                                message = response.message
                            });

                            OnUserResponseUpdated?.Invoke(userReponse);
                        }
                    });
                }
                else {
                    onComplete?.Invoke(new Response<UserResponse> {
                        result = Result.Failure,
                        content = default,
                        message = response.message
                    });
                }
            });
        }

        /// <summary>
        /// Gets the current user's guilds from the Discord API. Requires the necessary OAuth2 scope.
        /// This method is not used by the library, but is provided for convenience. Untested.
        /// </summary>
        /// <param name="accessToken">A valid Access Token</param>
        /// <param name="onComplete">
        /// A Response Object. Contains the UserResponse and well as Results, and Message containing any
        /// error information.
        /// </param>
        public static void GetGuilds(string accessToken, Action<Response<List<GuildResponse>>> onComplete) {
            var headers = new Dictionary<string, string> {
                { "Authorization", $"Bearer {accessToken}" }
            };

            Get(HttpsDiscordComApiV10UsersMeGuilds, headers, onComplete);
        }

        private static void GetAvatar(string accessToken, InternalUserResponse userInfo,
            Action<Result, Texture2D> onComplete) {
            if (userInfo == null || string.IsNullOrEmpty(accessToken)) {
                onComplete?.Invoke(Result.Failure, null);
                return;
            }

            if (_avatar.Item2 != null && _avatar.Item1 == userInfo.id) {
                onComplete?.Invoke(Result.Success, _avatar.Item2);
                return;
            }

            if (_avatar.Item2 != null) {
                Object.Destroy(_avatar.Item2);
                _avatar = (null, null);
            }
            
            var avatarURL = string.Format(HttpsCdnDiscordAppComAvatarsPNG, userInfo.id, userInfo.avatar);
            
            //If the user has no avatar, use their default one, which is based on their discriminator mod 5
            if (string.IsNullOrEmpty(userInfo.avatar) || userInfo.avatar == "default") {
                var avatarIndex = int.Parse(userInfo.discriminator) % 5;
                userInfo.avatar = "default";
                avatarURL = string.Format(HttpsCdnDiscordAppComEmbedAvatarsPNG, avatarIndex);
                if (printDebug) {
                    Debug.Log($"{EasyDiscordColoredName} User has no avatar, getting their default avatar");
                }
            }

            //Get user avatar from avatar url
            var webRequest = new UnityWebRequest(avatarURL, "GET");
            webRequest.downloadHandler = new DownloadHandlerTexture();
            var requestOp = webRequest.SendWebRequest();

            requestOp.completed += op => {
                if (webRequest.result != UnityWebRequest.Result.Success) {
                    if (printDebug) {
                        Debug.Log($"{EasyDiscordColoredName} Failed to get user avatar");
                    }

                    onComplete?.Invoke(Result.Failure, null);
                    return;
                }

                _avatar.Item1 = userInfo.id;
                _avatar.Item2 = DownloadHandlerTexture.GetContent(webRequest);
                onComplete?.Invoke(Result.Success, _avatar.Item2);
            };
        }

        /// <summary>
        /// Saves the current access token information to PlayerPrefs.
        /// </summary>
        /// <param name="accessTokenResponse">A valid Access Token Response object</param>
        public static void SaveAccessTokenToPlayerPrefs(AccessTokenResponse accessTokenResponse) {
            PlayerPrefs.SetString("EasyDiscord.AccessToken", accessTokenResponse.access_token);
            PlayerPrefs.SetString("EasyDiscord.RefreshToken", accessTokenResponse.refresh_token);
            PlayerPrefs.SetString("EasyDiscord.ExpiresIn",
                accessTokenResponse.expires_in.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("EasyDiscord.TokenType", accessTokenResponse.token_type);
            PlayerPrefs.SetString("EasyDiscord.Scope", accessTokenResponse.scope);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Clears the current access token information from PlayerPrefs.
        /// </summary>
        public static void ClearAccessTokenFromPlayerPrefs() {
            PlayerPrefs.DeleteKey("EasyDiscord.AccessToken");
            PlayerPrefs.DeleteKey("EasyDiscord.RefreshToken");
            PlayerPrefs.DeleteKey("EasyDiscord.ExpiresIn");
            PlayerPrefs.DeleteKey("EasyDiscord.TokenType");
            PlayerPrefs.DeleteKey("EasyDiscord.Scope");
            PlayerPrefs.Save();
        }

        private static void LoadAccessTokenFromPlayerPrefs(DiscordSettings settings,
            Action<Response<AccessTokenResponse>> onComplete) {
            var accessToken = PlayerPrefs.GetString("EasyDiscord.AccessToken");
            var refreshToken = PlayerPrefs.GetString("EasyDiscord.RefreshToken");
            var expiresIn = PlayerPrefs.GetString("EasyDiscord.ExpiresIn");
            var tokenType = PlayerPrefs.GetString("EasyDiscord.TokenType");
            var scope = PlayerPrefs.GetString("EasyDiscord.Scope");
            
            //Validate that all the required information is present
            if (!DateTime.TryParse(expiresIn, out var expires) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken) ||
                string.IsNullOrEmpty(expiresIn) || string.IsNullOrEmpty(tokenType) ||
                string.IsNullOrEmpty(scope)) {
                onComplete?.Invoke(new Response<AccessTokenResponse> {
                    result = Result.Failure,
                    content = default,
                    message = "Access Token information is missing from PlayerPrefs"
                });
                return;
            }
            
            var storedToken = new AccessTokenResponse {
                access_token = accessToken,
                refresh_token = refreshToken,
                expires_in = expires,
                token_type = tokenType,
                scope = scope
            };

            //Check if the token is expired
            if (storedToken.expires_in < DateTime.Now) {
                if (printDebug) {
                    Debug.Log($"{EasyDiscordColoredName} Access token is expired, refreshing");
                }

                RefreshAccessToken(storedToken.refresh_token, settings, onComplete);
                return;
            }

            onComplete?.Invoke(new Response<AccessTokenResponse> {
                result = Result.Success,
                content = storedToken,
                message = "Loaded access token from PlayerPrefs"
            });
        }

        /// <summary>
        /// Opens the Discord OAuth2 login page in the default browser. This will eventually redirect to the redirect URL.
        /// If there is a valid access token in PlayerPrefs, this will be used instead and no browser authentication will be
        /// required.
        /// </summary>
        /// <param name="settings">The Discord settings</param>
        /// <param name="success">
        /// A Response Object. Contains the AccessTokenResponse and well as Results, and Message containing
        /// any error information.
        /// </param>
        public static void Authorize(DiscordSettings settings, Action<Response<AccessTokenResponse>> success = null) {
            //Try to authorized with cached token first
            AuthorizeFromCachedToken(settings, response => {
                if (response.result == Result.Success) {
                    success?.Invoke(response);
                    return;
                }
                
                //If there is no cached token, continue with the browser authentication
                var scopesString = GetScopesFormatted(settings);
                if (string.IsNullOrEmpty(scopesString)) {
                    if (printDebug) {
                        Debug.Log($"{EasyDiscordColoredName} No scopes were selected, using default scopes");
                    }

                    scopesString = "identify%20email";
                }

                var url = string.Format(HttpsDiscordComApiV10Authorize, settings.clientId, settings.redirectUri, scopesString);
                Application.OpenURL(url);
                Application.focusChanged += OnApplicationFocus;

                void OnApplicationFocus(bool hasFocus) {
                    if (hasFocus) {
                        Application.focusChanged -= OnApplicationFocus;
                        // Check if clipboard has content
                        var code = GUIUtility.systemCopyBuffer;
                        if (code.Length > 25) {
                            // Clear clipboard
                            GUIUtility.systemCopyBuffer = "";
                            if (code != null) {
                                // Try to get access token
                                GetAccessToken(code, settings, response => {
                                    if (response.result == Result.Success && response.content.expires_in != null) {
                                        var accessTokenResponse = new AccessTokenResponse {
                                            access_token = response.content.access_token,
                                            token_type = response.content.token_type,
                                            refresh_token = response.content.refresh_token,
                                            scope = response.content.scope,
                                            expires_in = DateTime.Now.AddSeconds(int.Parse(response.content.expires_in))
                                        };

                                        success?.Invoke(new Response<AccessTokenResponse> {
                                            result = Result.Success,
                                            content = accessTokenResponse,
                                            message = response.message
                                        });
                                    }
                                    else {
                                        if (printDebug) {
                                            Debug.Log(
                                                "<color=#7289DA>Easy Discord:</color> <color=#FF0000>Failed to get access token." + response.message + "</color>");
                                        }

                                        success?.Invoke(new Response<AccessTokenResponse> {
                                            result = Result.Failure,
                                            content = null,
                                            message = response.message
                                        });
                                    }
                                });
                            }
                        }
                        else {
                            if (printDebug) {
                                Debug.Log($"{EasyDiscordColoredName} No code found in clipboard");
                            }

                            success?.Invoke(new Response<AccessTokenResponse> {
                                result = Result.Failure,
                                content = default,
                                message = "No code found in clipboard"
                            });
                        }
                    }
                }
            });
        }
        
        public static void AuthorizeFromCachedToken(DiscordSettings settings, Action<Response<AccessTokenResponse>> success = null) {
            //Try to load the access token from PlayerPrefs
            LoadAccessTokenFromPlayerPrefs(settings, response => {
                //Get all scopes and replace _ with .
                var scopes = Enum.GetValues(typeof(Scopes)).Cast<object>()
                    .Where(scope => settings.scopes.HasFlag((Scopes)scope)).Select(scope => scope.ToString().Replace("_", "."));
                
                //Make sure we have all the required scopes
                if (response.result == Result.Success && scopes.All(response.content.scope.Contains)) {
                    success?.Invoke(response);
                    return;
                }
                
                if (printDebug) {
                    Debug.Log($"{EasyDiscordColoredName} No cached token found");
                }

                success?.Invoke(new Response<AccessTokenResponse> {
                    result = Result.Failure,
                    content = default,
                    message = "No cached token found"
                });
            });
        }

        private enum Methods {
            Get,
            Post,
            Patch,
            Delete
        }

        #region WebRequestWrappers

        private static void Request<T>(string url, Methods method, Dictionary<string, string> body = null,
            Dictionary<string, string> headers = null, Action<Response<T>> success = null) {
            // Create request
            var webRequest = new UnityWebRequest(url, method.ToString().ToUpper());
            if (body != null) {
                var form = new WWWForm();
                foreach (var key in body.Keys) {
                    form.AddField(key, body[key]);
                }

                webRequest.uploadHandler = new UploadHandlerRaw(form.data);
            }

            webRequest.downloadHandler = new DownloadHandlerBuffer();
            if (headers != null) {
                foreach (var key in headers.Keys) {
                    webRequest.SetRequestHeader(key, headers[key]);
                }
            }

            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            // Send request
            var asyncOp = webRequest.SendWebRequest();
            asyncOp.completed += op => {
                // Check if request was successfu
                if (webRequest.result != UnityWebRequest.Result.Success) {
                    if (printDebug) {
                        Debug.Log(
                            $"{EasyDiscordColoredName} <color=#FF0000>Request failed: </color>" +
                            $"{webRequest.error} " +
                            $"{webRequest.downloadHandler.text}");
                    }

                    success?.Invoke(new Response<T> {
                        result = Result.Failure,
                        content = default,
                        message = webRequest.downloadHandler.text
                    });
                    return;
                }

                // Get response
                var response = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                success?.Invoke(new Response<T> {
                    result = Result.Success,
                    content = response,
                    message = webRequest.responseCode.ToString()
                });
            };
        }

        public static void Get<T>(string url, Dictionary<string, string> headers = null,
            Action<Response<T>> success = null) {
            Request(url, Methods.Get, null, headers, success);
        }

        public static void Post<T>(string url, Dictionary<string, string> body = null,
            Dictionary<string, string> headers = null, Action<Response<T>> success = null) {
            Request(url, Methods.Post, body, headers, success);
        }

        public static void Patch<T>(string url, Dictionary<string, string> body = null,
            Dictionary<string, string> headers = null, Action<Response<T>> success = null) {
            Request(url, Methods.Patch, body, headers, success);
        }

        public static void Delete<T>(string url, Dictionary<string, string> headers = null,
            Action<Response<T>> success = null) {
            Request(url, Methods.Delete, null, headers, success);
        }

        #endregion
    }

    [Serializable]
    public struct DiscordSettings {
        public string clientId;
        public string clientSecret;
        public string redirectUri;
        public Scopes scopes;

        public DiscordSettings(string clientId, string clientSecret, string redirectUri, Scopes scopes) {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.redirectUri = redirectUri;
            this.scopes = scopes;
        }
    }

    public struct Response<T> {
        public Result result;
        public T content;
        public string message;
    }

    public enum Result {
        Success,
        Failure,
        Ok,
        ServiceUnavailable,
        InvalidVersion,
        LockFailed,
        InternalError,
        InvalidPayload,
        InvalidCommand,
        InvalidPermissions,
        NotFetched,
        NotFound,
        Conflict,
        InvalidSecret
    }
}