﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetInventory
{
    public sealed class TagSelectionUI : PopupWindowContent
    {
        private List<AssetInfo> _assetInfo;
        private List<Tag> _tags;
        private string _newTag;
        private Vector2 _scrollPos;
        private bool _firstRunDone;
        private SearchField SearchField => _searchField = _searchField ?? new SearchField();
        private SearchField _searchField;
        private TagAssignment.Target _target;
        private Action _onChange;

        public void Init(TagAssignment.Target target, Action onChange = null)
        {
            _target = target;
            _onChange = onChange;
            _tags = Tagging.LoadTags();
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(220, AI.Config.tagListHeight);
        }

        public void SetAssets(List<AssetInfo> infos)
        {
            _assetInfo = infos;
        }

        public override void OnGUI(Rect rect)
        {
            if (_assetInfo == null) return;
            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return && !string.IsNullOrWhiteSpace(_newTag))
            {
                if (_newTag.Contains('/')) // prevent creating tags with slashes, as they are used for subfolders
                {
                    EditorUtility.DisplayDialog("Invalid Tag", "Tags cannot contain slashes (/). Please use a different name.", "OK");
                }
                else
                {
                    Tagging.AddAssignments(_assetInfo, _newTag, _target, true);
                    _newTag = "";
                }
            }
            GUILayout.BeginHorizontal();
            _newTag = SearchField.OnGUI(_newTag, GUILayout.ExpandWidth(true));
            if (GUILayout.Button(EditorGUIUtility.IconContent("Settings", "|Manage Tags").image, EditorStyles.label))
            {
                TagsUI tagsUI = TagsUI.ShowWindow();
                tagsUI.Init();
            }
            GUILayout.EndHorizontal();
            if (_tags != null)
            {
                if (_tags.Count == 0)
                {
                    EditorGUILayout.HelpBox("No tags created yet. Use the textfield above to create the first tag.", MessageType.Info);
                }
                else
                {
                    _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true));
                    int shownItems = 0;
                    foreach (Tag tag in _tags)
                    {
                        // don't show already added tags (for case of only one item selected, otherwise assigning it to all)
                        switch (_target)
                        {
                            case TagAssignment.Target.Package:
                                if (_assetInfo.Count == 1 && _assetInfo[0].PackageTags.Any(t => t.TagId == tag.Id)) continue;
                                break;

                            case TagAssignment.Target.Asset:
                                if (_assetInfo.Count == 1 && _assetInfo[0].AssetTags.Any(t => t.TagId == tag.Id)) continue;
                                break;
                        }
                        if (!string.IsNullOrWhiteSpace(_newTag) && !tag.Name.ToLowerInvariant().Contains(_newTag.ToLowerInvariant())) continue;
                        shownItems++;

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(8);
                        UIStyles.DrawTag(tag.Name, tag.GetColor(), () =>
                        {
                            Tagging.AddAssignments(_assetInfo, tag.Name, _target, true);
                            _onChange?.Invoke();
                        }, UIStyles.TagStyle.Add);
                        if (!string.IsNullOrWhiteSpace(tag.Hotkey))
                        {
                            EditorGUILayout.LabelField($"Alt+{tag.Hotkey}", UIStyles.greyMiniLabel);
                        }
                        GUILayout.EndHorizontal();
                    }
                    if (shownItems == 0)
                    {
                        if (string.IsNullOrWhiteSpace(_newTag))
                        {
                            EditorGUILayout.HelpBox("All existing tags were assigned already. Use the textfield above to create additional tags.", MessageType.Info);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Press RETURN to create a new tag", MessageType.Info);
                        }
                    }
                    GUILayout.EndScrollView();
                }
            }
            if (!_firstRunDone)
            {
                SearchField.SetFocus();
                _firstRunDone = true;
            }
        }
    }
}