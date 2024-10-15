namespace Howl
{
    using UnityEngine.UIElements;

    public static class EditorUtil
    {
        public static VisualElement AddSeparator(this VisualElement root)
        {
            var element = new VisualElement();
            element.AddToClassList("separator");
            root.Add(element);
            return element;
        }
    }
}