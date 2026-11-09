using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    [CustomEditor(typeof(FieldSO))]
    public class FieldSOEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            var fieldIDProp = serializedObject.FindProperty("fieldID");
            var nameProp = serializedObject.FindProperty("label");
            var typeProp = serializedObject.FindProperty("fieldType");
            var dropdownProp = serializedObject.FindProperty(nameof(FieldSO.enumChoices));

            var fieldIDField = new PropertyField(fieldIDProp);
            var nameField = new PropertyField(nameProp, "Label");
            nameField.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                var textField = nameField.Q<TextField>();
                if (textField != null)
                    textField.isDelayed = true;
            });
            var typeField = new PropertyField(typeProp);
            var dropdownField = new PropertyField(dropdownProp, "Choices");

            container.Add(fieldIDField);
            container.Add(nameField);
            container.Add(typeField);
            container.Add(dropdownField);

            void Refresh()
            {
                var type = (CustomFieldType)typeProp.enumValueIndex;
                dropdownField.style.display = type == CustomFieldType.Enum ? DisplayStyle.Flex : DisplayStyle.None;
            }

            typeField.RegisterValueChangeCallback(evt => Refresh());
            Refresh();

            return container;
        }
    }
}