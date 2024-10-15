namespace Howl
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(ColorPaletteBinding), true)]
    public class ColorPaletteBindingEditor : Editor
    {
        [SerializeField] private StyleSheet _styleSheet;
        private ColorPaletteBinding _binding;

        private VisualElement _root;
        private readonly List<VisualElement> _bindingDropdowns = new();

        private readonly List<(SerializedProperty Property, VisualElement Swatch)> _swatches = new();

        private void OnEnable()
        {
            _binding = (ColorPaletteBinding)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            _root = new VisualElement();
            _root.styleSheets.Add(_styleSheet);

            // Add source field
            var collectionField = new PropertyField();
            collectionField.bindingPath = nameof(_binding.Source);
            collectionField.RegisterValueChangeCallback(OnCollectionChanged);
            _root.Add(collectionField);

            RebuildBindings(_root);

            return _root;
        }

        private void RebuildBindings(VisualElement root)
        {
            _swatches.Clear();
            _bindingDropdowns.ForEach(x => x.RemoveFromHierarchy());

            if (_binding.Source == null)
                return;

            foreach (var colorFunc in _binding.GetColorFunctions())
            {
                var layout = new VisualElement();
                layout.style.flexDirection = FlexDirection.Row;

                var dropdown = new DropdownField(colorFunc.FieldName, _binding.Source.ColorNames, 0);
                var boundProp = serializedObject.FindProperty(colorFunc.BindingPath);
                dropdown.BindProperty(boundProp);
                dropdown.RegisterValueChangedCallback(UpdateColors);
                dropdown.style.flexGrow = 1;

                // dict of swatch to binding - change bg color on UpdateColors
                var colorSwatch = new VisualElement();
                colorSwatch.AddToClassList("swatch");
                _swatches.Add((boundProp, colorSwatch));

                layout.Add(dropdown);
                layout.Add(colorSwatch);

                root.Add(layout);
                _bindingDropdowns.Add(layout);
            }
        }

        private void UpdateColors(ChangeEvent<string> evt)
        {
            foreach (var swatch in _swatches)
            {
                swatch.Swatch.style.backgroundColor = _binding.Source.GetColor(swatch.Property.stringValue);
            }

            _binding.UpdateColors();
        }

        private void OnCollectionChanged(SerializedPropertyChangeEvent evt)
        {
            RebuildBindings(_root);
        }
    }

    // [CustomEditor(typeof(ColorPaletteBinding), true)]
    // public class ColorPaletteBindingEditor : Editor
    // {
    //     public override void OnInspectorGUI()
    //     {
    //         var binder = (ColorPaletteBinding)target;
    //
    //         EditorGUI.BeginChangeCheck();
    //         var palette = (ColorCollection)EditorGUILayout.ObjectField("Source",
    //             binder.Source, typeof(ColorCollection), false);
    //
    //         if (EditorGUI.EndChangeCheck())
    //         {
    //             Undo.RecordObject(target, "Change Color Source");
    //             binder.Source = palette;
    //             binder.UpdateColor();
    //         }
    //
    //         if(palette == null)
    //             return;
    //
    //         EditorGUI.BeginChangeCheck();
    //         var options = palette.Colors.Select(x => x.Name).ToArray();
    //         foreach (var functions in binder.GetColorFunctions())
    //         {
    //             int selectedIndex = palette.Colors.FindIndex(x => x.Id == functions.GetColor());
    //             selectedIndex = EditorGUILayout.Popup(functions.FieldName, selectedIndex, options);
    //             if(selectedIndex < 0)
    //                 continue;
    //
    //             if (EditorGUI.EndChangeCheck())
    //             {
    //                 Undo.RecordObject(target, "Change Bound Color");
    //                 functions.SetColor(palette.Colors[selectedIndex].Id);
    //                 binder.UpdateColor();
    //             }
    //         }
    //     }
    // }
}