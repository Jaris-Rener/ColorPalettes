namespace Howl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu]
    public class ColorCollection : ScriptableObject
    {
        [SerializeField] private int _activeIndex;

        public List<ColorPalette> Palettes = new();
        public List<string> ColorNames = new();


#if UNITY_EDITOR
        private void Awake()
        {
            if (Palettes.Count <= 0)
                CreatePalette();
        }

        private void OnValidate()
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            Palettes = assets.OfType<ColorPalette>().ToList();

            if (Palettes.Count <= 0)
                CreatePalette();
        }

        [ContextMenu("New Color Palette")]
        public ColorPalette CreatePalette()
        {
            var palette = CreateInstance<ColorPalette>();
            palette.name = GetNextName("New Color Palette");
            palette.Base = this;
            Palettes.Add(palette);
            AssetDatabase.AddObjectToAsset(palette, this);
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(palette);

            Undo.RegisterCreatedObjectUndo(palette, "Create Palette");
            return palette;
        }

        private string GetNextName(string candidateName, int iteration = 0)
        {
            var existing = Palettes.Find(x => x.name == candidateName);
            if (!existing)
            {
                return iteration == 0
                    ? candidateName
                    : $"{candidateName} ({iteration})";
            }

            if (iteration >= 100)
                return Guid.NewGuid().ToString();

            return GetNextName(candidateName, iteration + 1);
        }
#endif
        public Color GetColor(string key)
        {
            return GetActivePalette().GetEntry(key);
        }

        public ColorPalette GetActivePalette()
        {
            return Palettes.Count <= 0 ? null : Palettes[_activeIndex];
        }

        public void SetActivePalette(ColorPalette palette)
        {
            _activeIndex = Palettes.IndexOf(palette);
            ApplySchemeEverywhere();
        }

        public void ApplySchemeEverywhere()
        {
            var all = Resources.FindObjectsOfTypeAll<ColorPaletteBinding>();
            foreach (var obj in all)
            {
                if (obj.Source != this)
                    continue;

                obj.UpdateColors();
#if UNITY_EDITOR
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(obj.UntypedTarget);
#endif
            }
        }
    }
}