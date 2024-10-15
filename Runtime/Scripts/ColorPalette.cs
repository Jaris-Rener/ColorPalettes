namespace Howl
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class ColorLookupEntry
    {
        public string Key;
        public Color Value;

        public ColorLookupEntry(string key, Color color)
        {
            Key = key;
            Value = color;
        }
    }

    [Serializable]
    public class ColorLookup
    {
        public List<ColorLookupEntry> Entries = new();

        public bool ContainsKey(string key)
        {
            var entry = Entries.Find(x => x.Key == key);
            return entry != null;
        }

        public void Add(string key, Color color)
        {
            Entries.Add(new ColorLookupEntry(key, color));
        }

        public bool TryGetValue(string key, out Color color)
        {
            var entry = Entries.Find(x => x.Key == key);
            color = entry?.Value ?? default;
            return entry != null;
        }

        public Color this[string key]
        {
            get
            {
                var entry = Entries.Find(x => x.Key == key);
                return entry?.Value ?? default;
            }
            set
            {
                var entry = Entries.Find(x => x.Key == key);
                if (entry != null)
                    entry.Value = value;
                else
                    Entries.Add(new ColorLookupEntry(key, value));
            }
        }
    }

    public class ColorPalette : ScriptableObject
    {
        public ColorCollection Base;
        public ColorLookup Colors;

#if UNITY_EDITOR
        private void OnValidate()
        {
            Validate();
        }

        public void Validate()
        {
            bool dirty = false;
            foreach (var colorName in Base.ColorNames)
            {
                if (!Colors.ContainsKey(colorName))
                {
                    Colors.Add(colorName, Color.white);
                    dirty = true;
                }
            }

            for (var i = Colors.Entries.Count - 1; i >= 0; i--)
            {
                var entry = Colors.Entries[i];
                if (!Base.ColorNames.Contains(entry.Key))
                {
                    Colors.Entries.Remove(entry);
                    dirty = true;
                }
            }

            if (Base == null)
            {
                var assetPath = AssetDatabase.GetAssetPath(this);
                Base = (ColorCollection)AssetDatabase.LoadMainAssetAtPath(assetPath);
                dirty = true;
            }

            if (dirty)
                EditorUtility.SetDirty(this);
        }
#endif

        public Color GetEntry(string guid)
        {
            if (Colors.TryGetValue(guid, out var color))
                return color;

            Colors[guid] = Color.white;
            return color;
        }

        public void SetColor(string id, Color color)
        {
            Colors[id] = color;
        }
    }
}