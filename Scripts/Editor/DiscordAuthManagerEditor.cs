using System.Collections.Generic;
using Plugins.EasyDiscord.Scripts;
using Plugins.EasyDiscord.Scripts.Objects;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace EasyDiscord.Editor {
    [CustomEditor(typeof(EasyDiscordSignIn))]
    public class DiscordAuthManagerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var titleStyle = new GUIStyle(EditorStyles.label) {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            titleStyle.normal.textColor = new Color(114f / 255f, 137f / 255f, 218f / 255f);
            GUILayout.Space(10);
            GUILayout.Box("Easy Discord Sign-in",
                titleStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Box("Version 1.0.0",
                new GUIStyle("Label") { alignment = TextAnchor.MiddleCenter, wordWrap = true },
                GUILayout.ExpandWidth(true), GUILayout.Height(20));
            var myScript = (EasyDiscordSignIn)target;

            GUILayout.Space(10);

            var settings = myScript.settings;
            settings.clientId = EditorGUILayout.TextField("Client ID", settings.clientId);
            settings.clientSecret = EditorGUILayout.TextField("Client Secret", settings.clientSecret);
            settings.redirectUri = EditorGUILayout.TextField("Redirect URI", settings.redirectUri);
            settings.scopes = (Scopes)EditorGUILayout.EnumFlagsField("Scopes", settings.scopes);

            GUILayout.Space(10);

            myScript.printDebugLogs = EditorGUILayout.Toggle("Print Debug Logs", myScript.printDebugLogs);
            if (GUILayout.Button("Open Discord Developer Portal")) {
                Application.OpenURL("https://discord.com/developers/applications");
            }

            myScript.settings = settings;
            var user = myScript.User;
            if (user != null && !string.IsNullOrEmpty(user.id)) {
                GUILayout.Space(20);
                GUILayout.Label("User Information", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();

                GUILayout.Label("Username: " + user.username + "#" + user.discriminator);
                GUILayout.Label("Email: " + user.email);
                GUILayout.Label("ID: " + user.id);
                GUILayout.EndVertical();
                GUILayout.Box(user.avatar, GUILayout.Width(64), GUILayout.Height(64));
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);
            var events = new List<UnityEvent>();
            var eventNames = new List<string>();
            var fields = myScript.GetType().GetFields();
            foreach (var field in fields) {
                if (field.FieldType == typeof(UnityEvent)) {
                    events.Add((UnityEvent)field.GetValue(myScript));
                    eventNames.Add(field.Name);
                }
            }

            myScript.showEvents = EditorGUILayout.Foldout(myScript.showEvents, "Events");
            if (myScript.showEvents) {
                for (var i = 0; i < events.Count; i++) {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(eventNames[i]));
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}