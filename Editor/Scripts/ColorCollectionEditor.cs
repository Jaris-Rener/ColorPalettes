namespace Howl
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Button = UnityEngine.UIElements.Button;

    [CustomEditor(typeof(ColorCollection))]
    public class ColorCollectionEditor : Editor
    {
        [SerializeField] private StyleSheet _styleSheet;

        private ColorCollection _collection;
        private InspectorElement _paletteInspector;
        private VisualElement _palettesRoot;
        private VisualElement _colorsRoot;

        private ColorPalette _curPalette;
        private SerializedObject _curPaletteObj;
        private DropdownField _palettesDropdown;
        private List<string> _paletteChoices;
        private TextField _nameField;

        public override VisualElement CreateInspectorGUI()
        {
            _collection = (ColorCollection)target;
            _curPalette = _collection.GetActivePalette();
            if (_curPalette != null)
                _curPaletteObj = new SerializedObject(_curPalette);

            _paletteChoices = _collection.Palettes.Select(x => x.name).ToList();

            var root = new VisualElement();
            root.styleSheets.Add(_styleSheet);

            var tabbedView = new TabView();

            // Palettes Tab
            var palettesTab = new Tab("Palettes");
            _palettesRoot = palettesTab.contentContainer;
            tabbedView.Add(palettesTab);

            SetupPalettesTab();

            _paletteInspector = new InspectorElement(_curPaletteObj);
            _palettesRoot.Add(_paletteInspector);

            BindPalette(_curPalette);

            // Colors Tab
            var colorsTab = new Tab("Colors");
            _colorsRoot = colorsTab.contentContainer;
            tabbedView.Add(colorsTab);

            SetupColorsTab();

            root.Add(tabbedView);
            return root;
        }

        private void SetupColorsTab()
        {
            var prop = serializedObject.FindProperty(nameof(ColorCollection.ColorNames));
            var colors = new PropertyField(prop);
            colors.AddToClassList("tab");
            colors.RegisterValueChangeCallback(OnColorsChanged);
            colors.BindProperty(prop);
            _colorsRoot.Add(colors);
        }

        private void OnColorsChanged(SerializedPropertyChangeEvent evt)
        {
            _collection.Palettes.ForEach(x => x.Validate());
        }

        private void SetupPalettesTab()
        {
            var dropdownLyt = new VisualElement();
            dropdownLyt.style.flexDirection = FlexDirection.Row;

            // var paletteNames = _collection.Palettes.Select(x => x.name).ToList();
            _palettesDropdown = new DropdownField();
            _palettesDropdown.AddToClassList("dropdown");
            _palettesDropdown.choices = _paletteChoices;
            _palettesDropdown.value = _curPalette != null ? _curPalette.name : string.Empty;
            _palettesDropdown.AddToClassList("tab");
            _palettesDropdown.RegisterValueChangedCallback(OnPaletteChanged);

            var palettesAddBtn = new Button(AddPalette);
            palettesAddBtn.AddToClassList("button");
            palettesAddBtn.text = "Create Palette";

            var buttonsLyt = new VisualElement();
            buttonsLyt.style.flexDirection = FlexDirection.Row;

            var activateButton = new Button(ActivateCurrentPalette);
            activateButton.AddToClassList("button-primary");
            activateButton.text = "Activate Palette";
            var deleteButton = new Button(PromptDeleteCurrentPalette);
            deleteButton.AddToClassList("button-danger");
            deleteButton.text = "Delete Palette";

            _nameField = new TextField("Palette Name");
            _nameField.AddToClassList("input-field");
            _nameField.value = _curPalette != null ? _curPalette.name : string.Empty;
            _nameField.RegisterValueChangedCallback(OnRenamePalette);

            buttonsLyt.Add(activateButton);
            buttonsLyt.Add(deleteButton);

            dropdownLyt.Add(_palettesDropdown);
            dropdownLyt.Add(palettesAddBtn);

            _palettesRoot.Add(dropdownLyt);
            _palettesRoot.Add(buttonsLyt);
            _palettesRoot.AddSeparator();
            _palettesRoot.Add(_nameField);
            _palettesRoot.AddSeparator();
        }

        private void OnRenamePalette(ChangeEvent<string> evt)
        {
            _curPaletteObj.ApplyModifiedProperties();
            AssetDatabase.SaveAssetIfDirty(_curPalette);
        }

        private void PromptDeleteCurrentPalette()
        {
            if (_collection.Palettes.Count <= 1)
                return;

            if (_curPalette == null)
                return;

            var ok = EditorUtility.DisplayDialog("Delete Palette", "Are you sure you want to delete this palette?", "Delete", "Cancel");
            if (ok)
                DeleteCurrentPalette();
        }

        private void DeleteCurrentPalette()
        {
            if (_curPalette != null)
            {
                _paletteChoices.Remove(_curPalette.name);
                _collection.Palettes.Remove(_curPalette);
                Undo.DestroyObjectImmediate(_curPalette);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_collection));

                _palettesDropdown.value = _paletteChoices[0];
            }
        }

        private void ActivateCurrentPalette()
        {
            if (_curPalette != null)
                _collection.SetActivePalette(_curPalette);
        }

        private void AddPalette()
        {
            var palette = _collection.CreatePalette();
            _paletteChoices.Add(palette.name);

            _palettesDropdown.value = palette.name;
        }

        private void OnPaletteChanged(ChangeEvent<string> evt)
        {
            var palette = _collection.Palettes.Find(x => x.name == evt.newValue);
            BindPalette(palette);
        }

        private void BindPalette(ColorPalette palette)
        {
            _curPalette = palette;

            if (palette != null)
                _curPaletteObj = new SerializedObject(palette);

            // Setup bindings

            if (_curPaletteObj != null)
            {
                if (_paletteInspector == null)
                {
                    _paletteInspector = new InspectorElement(_curPaletteObj);
                    _palettesRoot.Add(_paletteInspector);
                }

                _paletteInspector.Unbind();
                _paletteInspector.Bind(_curPaletteObj);
            }
            else
            {
                _paletteInspector.RemoveFromHierarchy();
                _paletteInspector = null;
            }

            if (_nameField != null)
            {
                _nameField.Unbind();
                if (_curPaletteObj != null)
                    _nameField.BindProperty(_curPaletteObj.FindProperty("m_Name"));
            }
        }
    }
}