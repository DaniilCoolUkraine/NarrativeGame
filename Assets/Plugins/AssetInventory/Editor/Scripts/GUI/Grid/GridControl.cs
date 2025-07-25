using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetInventory
{
    internal sealed class GridControl
    {
        public event Action<AssetInfo> OnDoubleClick;

        public IEnumerable<AssetInfo> packages;
        private GUIContent[] _contents;
        public GUIContent[] contents
        {
            get => _contents;
            set
            {
                if (_contents != null)
                {
                    foreach (GUIContent content in _contents)
                    {
                        if (content != null && content.image != null)
                        {
                            // Skip built-in Unity icons which shouldn't be destroyed
                            if (!AssetDatabase.GetAssetPath(content.image).StartsWith("Library/"))
                            {
                                UnityEngine.Object.DestroyImmediate(content.image);
                            }
                        }
                    }
                }
                _contents = value;
            }
        }
        public List<AssetInfo> selectionItems;
        public int selectionCount;
        public int selectionTile;
        public int selectionPackageCount;
        public long selectionSize;
        public int noTextBelow;
        public bool enlargeTiles;
        public bool centerTiles;
        public bool IsMouseOverGrid;

        private GUIContent[] Selection
        {
            get
            {
                if (_selection == null || (contents != null && _selection.Length != contents.Length))
                {
                    _selection = new GUIContent[contents != null ? contents.Length : 0];
                    _selection.Populate(UIStyles.emptyTileContent);
                    MarkGridSelection();
                }
                return _selection;
            }
        }
        private Func<AssetInfo, string> _textGenerator;
        private GUIContent[] _selection;
        private int _selectionMin;
        private int _selectionMax;
        private int _lastSelectionTile;
        private List<AssetInfo> _allPackages;
        private Action _bulkHandler;
        private Rect _lastRect;

        public void Init(List<AssetInfo> allPackages, IEnumerable<AssetInfo> visiblePackages, Action bulkHandler, Func<AssetInfo, string> textGenerator = null)
        {
            packages = visiblePackages;
            _textGenerator = textGenerator;
            _allPackages = allPackages;
            _bulkHandler = bulkHandler;
            _selection = new GUIContent[contents.Length];
            _selection.Populate(UIStyles.emptyTileContent);
            MarkGridSelection();
            CalculateBulkSelection();
        }

        public void Draw(float width, int inspectorCount, int tileSize, GUIStyle tileStyle, GUIStyle selectedTileStyle)
        {
            float actualWidth = width - UIStyles.INSPECTOR_WIDTH * inspectorCount - UIStyles.BORDER_WIDTH;
            int cells = Mathf.Clamp(Mathf.FloorToInt(actualWidth / (tileSize + AI.Config.tileMargin)), 1, 99);
            if (cells < 2) cells = 2;

            if (enlargeTiles)
            {
                // enlarge tiles dynamically so they take the full width
                tileSize = Mathf.FloorToInt((actualWidth - cells * AI.Config.tileMargin) / cells);
            }

            tileStyle.fixedHeight = tileSize;
            tileStyle.fixedWidth = tileSize;
            tileStyle.margin = new RectOffset(AI.Config.tileMargin, AI.Config.tileMargin, AI.Config.tileMargin, AI.Config.tileMargin); // set again due to initial style only being set once so changes would not reflect
            selectedTileStyle.fixedHeight = tileStyle.fixedHeight + tileStyle.margin.top;
            selectedTileStyle.fixedWidth = tileStyle.fixedWidth + tileStyle.margin.left;

            if (_textGenerator != null && contents != null && contents.Length > 0)
            {
                // remove text if tiles are too small
                if (tileSize < noTextBelow)
                {
                    if (!string.IsNullOrEmpty(contents[0].text))
                    {
                        contents.ForEach(c => c.text = string.Empty);
                    }
                }

                // create text on-demand if tiles are big enough
                if (tileSize >= noTextBelow)
                {
                    if (string.IsNullOrEmpty(contents[0].text))
                    {
                        for (int i = 0; i < contents.Length; i++)
                        {
                            contents[i].text = _textGenerator(packages.ElementAt(i));
                        }
                    }
                }
            }

            GUILayout.BeginHorizontal();
            if (centerTiles) GUILayout.Space((actualWidth - tileSize * cells) / 2f);
            selectionTile = GUILayout.SelectionGrid(selectionTile, contents, cells, tileStyle);
            _lastRect = UIStyles.GetCurrentVisibleRect(); // GetLastRect would include invisible scroll area as well
            if (Event.current.type == EventType.Repaint)
            {
                IsMouseOverGrid = _lastRect.Contains(Event.current.mousePosition);
            }

            if (selectionCount > 1)
            {
                // draw selection on top if there are more than one selected, otherwise don't for performance
                // use real last rect to support scrolling
                GUI.SelectionGrid(GUILayoutUtility.GetLastRect(), selectionTile, Selection, cells, selectedTileStyle);
            }
            GUILayout.EndHorizontal();

            if (Event.current.type == EventType.Layout)
            {
                // handle double-clicks
                if (Event.current.clickCount > 1)
                {
                    if (IsMouseOverGrid) OnDoubleClick?.Invoke(packages.ElementAt(selectionTile));
                }
            }
        }

        public void LimitSelection(int count)
        {
            if (selectionTile >= count) selectionTile = 0;
        }

        private void MarkGridSelection()
        {
            if (selectionTile >= Selection.Length) selectionTile = 0;
            if (selectionTile >= 0 && Selection.Length > 0) Selection[selectionTile] = UIStyles.selectedTileContent;
        }

        public void HandleMouseClicks()
        {
            if (selectionTile >= 0)
            {
                if (!Event.current.control && !Event.current.shift)
                {
                    // regular click, no ctrl/shift
                    Selection.Populate(UIStyles.emptyTileContent);
                    MarkGridSelection();

                    _selectionMin = selectionTile;
                    _selectionMax = selectionTile;
                }
                else if (Event.current.control)
                {
                    // toggle existing selection
                    Selection[selectionTile] = Selection[selectionTile] == UIStyles.selectedTileContent ? UIStyles.emptyTileContent : UIStyles.selectedTileContent;
                }
                else if (Event.current.shift)
                {
                    // shift click - add all between clicks
                    if (_selectionMin != -1 && _selectionMax != -1 && selectionTile >= _selectionMin && selectionTile <= _selectionMax)
                    {
                        Selection.Populate(UIStyles.emptyTileContent);
                        _selectionMin = Mathf.Min(_lastSelectionTile, selectionTile);
                        _selectionMax = Mathf.Max(_lastSelectionTile, selectionTile);
                    }
                    int minI = Mathf.Min(_lastSelectionTile, selectionTile);
                    int maxI = Mathf.Max(_lastSelectionTile, selectionTile);
                    if (minI < 0) minI = 0;
                    if (maxI < 0) maxI = 0;

                    if (Selection.Length > 0)
                    {
                        for (int i = minI; i <= maxI; i++)
                        {
                            Selection[i] = UIStyles.selectedTileContent;
                        }
                    }
                }

                _selectionMin = Mathf.Min(_selectionMin, selectionTile);
                _selectionMax = Mathf.Max(_selectionMax, selectionTile);
                _lastSelectionTile = selectionTile;

                CalculateBulkSelection();
            }
        }

        public void HandleKeyboardCommands()
        {
            if (Event.current.modifiers == EventModifiers.Control && Event.current.keyCode == KeyCode.A)
            {
                // select all
                Selection.Populate(UIStyles.selectedTileContent);
                MarkGridSelection();

                _selectionMin = 0;
                _selectionMax = selectionCount - 1;

                CalculateBulkSelection();
            }
        }

        private void CalculateBulkSelection()
        {
            selectionItems = Selection
                .Select((item, index) => item == UIStyles.selectedTileContent ? packages.ElementAt(index) : null)
                .Where(item => item != null)
                .ToList();
            selectionCount = selectionItems.Count;
            selectionSize = selectionItems.Sum(item => item.Size);
            selectionPackageCount = selectionItems.GroupBy(item => item.AssetId).Count();
            selectionItems.ForEach(info => info.CheckIfInProject());
            AI.ResolveParents(selectionItems, _allPackages);

            _bulkHandler?.Invoke();
        }

        public void DeselectAll()
        {
            selectionTile = 0;
            Selection.Populate(UIStyles.emptyTileContent);
            MarkGridSelection();
            CalculateBulkSelection();
        }

        public void Select(AssetInfo info)
        {
            selectionTile = packages.ToList().FindIndex(p => p.AssetId == info.AssetId);
            Selection.Populate(UIStyles.emptyTileContent);
            MarkGridSelection();

            _selectionMin = selectionTile;
            _selectionMax = selectionTile;

            CalculateBulkSelection();
        }
    }
}