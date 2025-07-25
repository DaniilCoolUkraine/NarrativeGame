﻿// reference for built-in icons: https://github.com/halak/unity-editor-icons
// new version: https://github.com/Doppelkeks/Unity-Editor-Icons/tree/2019.4

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AssetInventory
{
    public static class UIStyles
    {
        public enum TagStyle
        {
            Add = 0,
            Remove = 1,
            Neutral = 2,
            ColorSelect = 3
        }

        public const int BORDER_WIDTH = 30;
        public const int INSPECTOR_WIDTH = 300;
        public const int TAG_SIZE_SPACING = 20;
        public const int TAG_OUTER_MARGIN = 20;
        public const string INDENT = "  ";
        public const int INDENT_WIDTH = 8;
        public const float BIG_BUTTON_HEIGHT = 50f;

        public static readonly string[] FolderTypes = {"Unity Packages", "Media Folder", "Archives", "Dev Packages"};
        public static readonly string[] MediaTypes = {"-All Media-", "-All Files-", string.Empty, "Audio", "Images", "Models", string.Empty, "-Custom File Pattern-"};

        public static readonly Color errorColor = EditorGUIUtility.isProSkin ? new Color(1f, 0.5f, 0.5f) : Color.red;

        private static readonly GUIContent GUIText = new GUIContent();
        private static readonly GUIContent GUIImage = new GUIContent();
        private static readonly GUIContent GUITextImage = new GUIContent();

        private const int ENTRY_FONT_SIZE = 11;
        private const int ENTRY_FIXED_HEIGHT = ENTRY_FONT_SIZE + 7;
        private const int TOGGLE_FIXED_WIDTH = 10;

        public static readonly GUIStyle searchTile = CreateTileStyle();
        public static readonly GUIStyle packageTile = CreateTileStyle();
        public static readonly GUIStyle selectedSearchTile = CreateSelectedTileStyle();
        public static readonly GUIStyle selectedPackageTile = CreateSelectedTileStyle();

        public static readonly GUIStyle toggleButtonStyleNormal = new GUIStyle("button");
        public static readonly GUIStyle selectableLabel = new GUIStyle(EditorStyles.label)
        {
            wordWrap = true, margin = new RectOffset(0, 0, 0, 0), padding = new RectOffset(0, 0, 0, 0)
        };
        public static readonly GUIStyle toggleButtonStyleToggled = CreateToggledStyle();

        public static readonly GUIStyle wrappedLinkLabel = new GUIStyle(EditorStyles.linkLabel)
        {
            wordWrap = true
        };

        public static readonly GUIStyle greyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
        {
            alignment = TextAnchor.MiddleLeft
        };

        public static readonly GUIStyle wrappedButton = new GUIStyle(GUI.skin.button)
        {
            wordWrap = true
        };

        public static readonly GUIContent emptyTileContent = new GUIContent();
        public static readonly GUIContent selectedTileContent = new GUIContent
        {
            image = LoadTexture("asset-inventory-selected"),
            text = string.Empty,
            tooltip = string.Empty
        };
        public static readonly GUIStyle richText = new GUIStyle(EditorStyles.wordWrappedLabel)
        {
            richText = true
        };
        public static readonly GUIStyle miniLabelRight = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.MiddleRight
        };
        private static readonly Func<Rect> getVisibleRect;

        static UIStyles()
        {
            // cache the visible rect getter for performance
            Type clipType = typeof (GUI).Assembly.GetType("UnityEngine.GUIClip");
            PropertyInfo prop = clipType.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo getter = prop.GetGetMethod(true);
            getVisibleRect = (Func<Rect>)Delegate.CreateDelegate(typeof (Func<Rect>), getter);
        }

        public static Rect GetCurrentVisibleRect() => getVisibleRect();

        public static Texture2D LoadTexture(string name)
        {
            string asset = AssetDatabase.FindAssets("t:Texture2d " + name).FirstOrDefault();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(asset));
        }

        /*
        public static void SelectableLabel(GUIContent content, params GUILayoutOption[] options)
        {
            // This method is really buggy in Unity, the label will remain on screen and only change when defocusing etc.
            // Wrapping is completely broken
            EditorGUILayout.SelectableLabel(content.text, EditorStyles.wordWrappedLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MinHeight(EditorGUIUtility.singleLineHeight), GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight * 5));

            //Rect position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, selectableLabel, options);
            //EditorGUI.SelectableLabel(position, content.tooltip, selectableLabel);

            if (!string.IsNullOrEmpty(content.tooltip))
            {
                GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", content.tooltip), GUIStyle.none);
            }
        }
        */

        private static GUIStyle CreateToggledStyle()
        {
            GUIStyle baseStyle = new GUIStyle("button");
            baseStyle.normal.background = baseStyle.active.background;

            return baseStyle;
        }

        private static GUIStyle CreateTileStyle()
        {
            GUIStyle baseStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                imagePosition = ImagePosition.ImageAbove,
                wordWrap = true,
                margin = new RectOffset(AI.Config.tileMargin, AI.Config.tileMargin, AI.Config.tileMargin, AI.Config.tileMargin)
            };

            return baseStyle;
        }

        private static GUIStyle CreateSelectedTileStyle()
        {
            GUIStyle baseStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageOnly,
                overflow = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };

            baseStyle.normal.background = LoadTexture("asset-inventory-transparent");

            return baseStyle;
        }

        public static readonly GUIStyle tag = new GUIStyle(EditorStyles.miniButton)
        {
            border = new RectOffset(6, 6, 6, 6),
            fixedHeight = EditorGUIUtility.singleLineHeight + 2,
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(2, 2, 2, 2)
        };

        public static readonly GUIStyle entryStyle = new GUIStyle(EditorStyles.miniLabel) {fontSize = ENTRY_FONT_SIZE, fixedHeight = ENTRY_FIXED_HEIGHT};
        public static readonly GUIStyle toggleStyle = new GUIStyle(EditorStyles.toggle) {fixedWidth = TOGGLE_FIXED_WIDTH, fixedHeight = ENTRY_FIXED_HEIGHT};
        public static readonly GUIStyle whiteCenter = new GUIStyle {alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState {textColor = Color.white}};
        public static readonly GUIStyle blackCenter = new GUIStyle {alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState {textColor = Color.black}};
        public static readonly GUIStyle centerLabel = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
        public static readonly GUIStyle centerHeading = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold};
        public static readonly GUIStyle centeredWhiteMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel) {normal = new GUIStyleState {textColor = Color.white}};
        public static readonly GUIStyle rightLabel = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleRight};
        public static readonly GUIStyle centerLinkLabel = new GUIStyle(EditorStyles.linkLabel) {alignment = TextAnchor.MiddleCenter};
        public static readonly GUIStyle centerPopup = new GUIStyle(EditorStyles.popup) {alignment = TextAnchor.MiddleCenter};

        public static void DrawTag(TagInfo tagInfo, Action action = null)
        {
            DrawTag(tagInfo.Name, tagInfo.GetColor(), action, TagStyle.Remove);
        }

        public static void DrawTag(string name, Color color, Action action, TagStyle style)
        {
            Color oldColor = GUI.color;
            GUI.color = color;
            using (new EditorGUILayout.HorizontalScope(tag,
                       GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false),
                       GUILayout.Width(tag.CalcSize(Content(name)).x + (style != TagStyle.Neutral ? EditorGUIUtility.singleLineHeight : 0))))
            {
                GUI.color = GetHSPColor(color);
                GUIStyle readableText = ReadableText(color);

                GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                readableText.normal.textColor = GUI.color;

                switch (style)
                {
                    case TagStyle.Add:
                        if (GUILayout.Button("+ " + name, readableText, GUILayout.Height(EditorGUIUtility.singleLineHeight - 3)))
                        {
                            action?.Invoke();
                        }
                        break;

                    case TagStyle.Remove:
                        GUILayout.Label(name, readableText);
                        GUI.color = oldColor;
                        if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash", "|Remove Tag").image,
                                EditorStyles.label, GUILayout.Width(EditorGUIUtility.singleLineHeight),
                                GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                        {
                            action?.Invoke();
                        }
                        break;

                    case TagStyle.Neutral:
                        GUILayout.Label(name, readableText);
                        break;

                    case TagStyle.ColorSelect:
                        GUILayout.Label(name, readableText);
                        if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash", "|Remove Tag").image,
                                EditorStyles.label, GUILayout.Width(EditorGUIUtility.singleLineHeight),
                                GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                        {
                            action?.Invoke();
                        }
                        break;
                }
            }
            GUI.color = oldColor;
        }

        public static void DrawTag(Rect rect, string name, Color color, TagStyle style)
        {
            Color oldColor = GUI.color;
            GUI.color = color;
            // FIXME: broken, background not at correct position yet
            using (new EditorGUILayout.HorizontalScope(tag,
                       GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false),
                       GUILayout.Width(tag.CalcSize(Content(name)).x + (style != TagStyle.Neutral ? EditorGUIUtility.singleLineHeight : 0))))
            {
                GUI.color = GetHSPColor(color);
                switch (style)
                {
                    case TagStyle.Neutral:
                        GUI.Label(rect, name, ReadableText(color));
                        break;
                }
            }
            GUI.color = oldColor;
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2f;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public static GUILayoutOption GetLabelMaxWidth()
        {
            return GUILayout.MaxWidth(INSPECTOR_WIDTH - 115);
        }

        private static Color GetHSPColor(Color color)
        {
            // http://alienryderflex.com/hsp.html
            return 0.299 * color.r + 0.587 * color.g + 0.114 * color.b < 0.5f ? Color.white : new Color(0.1f, 0.1f, 0.1f);
        }

        public static GUIStyle ReadableText(Color color, bool wrap = false)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.normal.textColor = GetHSPColor(color);
            style.wordWrap = wrap;
            return style;
        }

        public static GUIStyle ColoredText(Color color)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.normal.textColor = color;
            return style;
        }

        public static void DrawProgressBar(float percentage, string text, params GUILayoutOption[] options)
        {
            Rect r = EditorGUILayout.BeginVertical(options);
            EditorGUI.ProgressBar(r, percentage, text);
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.EndVertical();
        }

        public static GUIContent Content(string text)
        {
            GUIText.image = null;
            GUIText.text = text;
            GUIText.tooltip = null;
            return GUIText;
        }

        public static GUIContent Content(string text, string tip, string ctrlText = null, string ctrlTip = null)
        {
            GUIText.image = null;
            GUIText.text = AI.ShowAdvanced() ? (string.IsNullOrEmpty(ctrlText) ? text : ctrlText) : text;
            GUIText.tooltip = AI.ShowAdvanced() ? (string.IsNullOrEmpty(ctrlTip) ? tip : ctrlTip) : tip;
            return GUIText;
        }

        public static GUIContent Content(Texture texture)
        {
            GUIImage.image = texture;
            GUIImage.text = null;
            GUIImage.tooltip = null;
            return GUIImage;
        }

        public static GUIContent Content(string text, Texture texture, string tip = null)
        {
            GUITextImage.image = texture;
            GUITextImage.text = " " + text; // otherwise text too close to image
            GUITextImage.tooltip = tip;
            return GUITextImage;
        }

        public static GUIContent IconContent(string name, string darkName, string tooltip = null)
        {
            if (EditorGUIUtility.isProSkin) return EditorGUIUtility.IconContent(darkName, tooltip);
            return EditorGUIUtility.IconContent(name, tooltip);
        }
    }
}