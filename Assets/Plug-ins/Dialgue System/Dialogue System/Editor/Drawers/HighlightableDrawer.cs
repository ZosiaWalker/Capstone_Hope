using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    [CustomPropertyDrawer(typeof(HighlightableAttribute))]
    public class HighlightableDrawer : PropertyDrawer
    {
        private readonly Color DuplicateColor = new Color(1f, 0.5f, 0.5f, 1f);

        //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        //{
        //    EditorGUI.BeginProperty(position, label, property);

        //    if (property.IsArrayElement())
        //    {
        //        SerializedProperty parentArray = property.GetParentArray();

        //        if (parentArray != null)
        //        {
        //            bool isDuplicate = CheckForDuplicates(parentArray, property);

        //            if (isDuplicate)
        //            {
        //                GUI.color = DuplicateColor;
        //            }
        //        }

        //        EditorGUI.PropertyField(position, property, label, true);
        //    }
        //    else
        //    {
        //        EditorGUI.PropertyField(position, property, label, true);
        //    }

        //    EditorGUI.EndProperty();
        //}

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Color prevColor = GUI.color; // Save the current GUI color

            if (property.IsArrayElement())
            {
                SerializedProperty parentArray = property.GetParentArray();
                if (parentArray != null)
                {
                    bool isDuplicate = CheckForDuplicates(parentArray, property);
                    if (isDuplicate)
                        GUI.color = DuplicateColor;
                }

                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

            GUI.color = prevColor;
            EditorGUI.EndProperty();
        }

        private bool CheckForDuplicates(SerializedProperty arrayProperty, SerializedProperty currentElementProperty)
        {
            int count = 0;
            object currentValue = GetPropertyValue(currentElementProperty);

            if (currentElementProperty.propertyType == SerializedPropertyType.ObjectReference && currentValue == null)
            {
                return false;
            }

            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                SerializedProperty element = arrayProperty.GetArrayElementAtIndex(i);
                object otherValue = GetPropertyValue(element);

                if (currentValue != null && otherValue != null && currentValue.Equals(otherValue))
                {
                    count++;
                }
                else if (currentValue == null && otherValue == null && currentElementProperty.propertyType == SerializedPropertyType.ObjectReference)
                {
                    count++;
                }
            }

            return count > 1;
        }

        private object GetPropertyValue(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return prop.intValue;
                case SerializedPropertyType.Boolean:
                    return prop.boolValue;
                case SerializedPropertyType.Float:
                    return prop.floatValue;
                case SerializedPropertyType.String:
                    return prop.stringValue;
                case SerializedPropertyType.Color:
                    return prop.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return prop.objectReferenceValue;
                case SerializedPropertyType.Enum:
                    return prop.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return prop.vector2Value;
                case SerializedPropertyType.Vector3:
                    return prop.vector3Value;
                case SerializedPropertyType.Vector4:
                    return prop.vector4Value;
                default:
                    return null;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }

    public static class SerializedPropertyExtensions
    {
        public static SerializedProperty GetParentArray(this SerializedProperty element)
        {
            string path = element.propertyPath;
            int lastDot = path.LastIndexOf('.');
            if (lastDot != -1)
            {
                string arrayPath = path.Substring(0, lastDot);
                SerializedProperty arrayProperty = element.serializedObject.FindProperty(arrayPath);
                if (arrayProperty != null && arrayProperty.isArray)
                {
                    return arrayProperty;
                }
            }
            return null;
        }

        public static bool IsArrayElement(this SerializedProperty element)
        {
            string path = element.propertyPath;
            int startBracket = path.LastIndexOf('[');
            int endBracket = path.LastIndexOf(']');
            return startBracket != -1 && endBracket != -1 && endBracket > startBracket;
        }
    }
}