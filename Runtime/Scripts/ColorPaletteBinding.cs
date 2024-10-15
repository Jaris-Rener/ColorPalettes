namespace Howl
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ColorPaletteBinding : MonoBehaviour
    {
        public ColorCollection Source;
        public abstract Component UntypedTarget { get; }

        public abstract void UpdateColors();

        public abstract IEnumerable<(string FieldName, string BindingPath)> GetColorFunctions();

        public void SetColorSource(ColorCollection collection)
        {
            Source = collection;
            UpdateColors();
        }
    }

    public abstract class ColorPaletteBinding<T> : ColorPaletteBinding where T : Component
    {
        public override Component UntypedTarget => Target;
        public abstract T Target { get; }
    }
}