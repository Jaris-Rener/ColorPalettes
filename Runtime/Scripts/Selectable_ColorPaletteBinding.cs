namespace Howl
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Selectable))]
    public class Selectable_ColorPaletteBinding : ColorPaletteBinding<Selectable>
    {
        public override Selectable Target => _dropdown == null
            ? _dropdown = GetComponent<Selectable>()
            : _dropdown;

        private Selectable _dropdown;

        public string NormalColorId = string.Empty;
        public string HighlightedColorId = string.Empty;
        public string PressedColorId = string.Empty;
        public string SelectedColorId = string.Empty;
        public string DisabledColorId = string.Empty;


        public override IEnumerable<(string FieldName, string BindingPath)> GetColorFunctions()
        {
            yield return ("Normal Color", nameof(NormalColorId));
            yield return ("Highlighted Color", nameof(HighlightedColorId));
            yield return ("Pressed Color", nameof(PressedColorId));
            yield return ("Selected Color", nameof(SelectedColorId));
            yield return ("Disabled Color", nameof(DisabledColorId));
        }

        public override void UpdateColors()
        {
            if (Source == null)
                return;

            var colors = Target.colors;
            var palette = Source.GetActivePalette();

            colors.normalColor = palette.GetEntry(NormalColorId);
            colors.highlightedColor = palette.GetEntry(HighlightedColorId);
            colors.pressedColor = palette.GetEntry(PressedColorId);
            colors.selectedColor = palette.GetEntry(SelectedColorId);
            colors.disabledColor = palette.GetEntry(DisabledColorId);
            Target.colors = colors;
        }
    }
}