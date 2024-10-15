namespace Howl
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(ColorPalette))]
    public class ColorPaletteEditor : Editor
    {
        private ColorPalette _palette;

        private void OnEnable()
        {
            _palette = target as ColorPalette;
            if (_palette != null)
                _palette.Validate();
        }

        public override VisualElement CreateInspectorGUI()
        {
            _palette = (ColorPalette)target;

            var root = new VisualElement();

            var colorsProp = serializedObject.FindProperty(nameof(ColorPalette.Colors));
            var entriesProp = colorsProp.FindPropertyRelative(nameof(ColorLookup.Entries));
            for (var i = 0; i < _palette.Colors.Entries.Count; i++)
            {
                var entry = _palette.Colors.Entries[i];
                var lyt = new VisualElement();
                lyt.style.flexDirection = FlexDirection.Row;

                var entryProp = entriesProp.GetArrayElementAtIndex(i);
                var colorProp = entryProp.FindPropertyRelative(nameof(ColorLookupEntry.Value));
                var colorField = new ColorField(entry.Key);
                colorField.style.flexGrow = 1;
                colorField.BindProperty(colorProp);

                lyt.Add(colorField);

                root.Add(lyt);
            }

            return root;
        }
    }
}