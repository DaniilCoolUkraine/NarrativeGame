﻿using System;
using SQLite;
using UnityEngine;

namespace AssetInventory
{
    [Serializable]
    public sealed class Tag : IEquatable<Tag>
    {
        public static Color DefaultColor = UnityEngine.Color.white;

        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        [Indexed] [Collation("NOCASE")] public string Name { get; set; }
        public string Color { get; set; }
        public bool FromAssetStore { get; set; }
        public string Hotkey { get; set; }  // Stores the keyboard shortcut (e.g. "1", "a", etc.)

        public Tag()
        {
        }

        public Tag(string name)
        {
            Name = name;
        }

        public Color GetColor()
        {
            return ColorUtility.TryParseHtmlString(Color, out Color toUse) ? toUse : DefaultColor;
        }

        public bool Equals(Tag other)
        {
            return other?.Name == Name;
        }

        public override string ToString()
        {
            return $"Tag '{Name}' ({Color})";
        }
    }
}