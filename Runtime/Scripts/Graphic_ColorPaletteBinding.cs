namespace Howl
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Graphic))]
    public class Graphic_ColorPaletteBinding : ColorPaletteBinding<Graphic>
    {
        public override Graphic Target => _graphic == null
            ? _graphic = GetComponent<Graphic>()
            : _graphic;

        private Graphic _graphic;

        public string SelectedColor = string.Empty;

        public override void UpdateColors()
        {
            if (Source != null)
                Target.color = Source.GetColor(SelectedColor);
        }

        public override IEnumerable<(string FieldName, string BindingPath)> GetColorFunctions()
        {
            yield return ("Bound Color", nameof(SelectedColor));
        }
    }
}