using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    [CustomPropertyDrawer(typeof(KeyValueSerializable<,>), true)]
    public class KeyValueSerializableDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyProp = property.FindPropertyRelative("Key");

            SerializedProperty listProperty = FindParentList(property);
            bool isDuplicate = false;

            if (listProperty != null)
            {
                var currentKey = GetPropertyValue(keyProp);
                int duplicates = 0;
                for (int i = 0; i < listProperty.arraySize; i++)
                {
                    var element = listProperty.GetArrayElementAtIndex(i);
                    var otherKeyProp = element.FindPropertyRelative("Key");
                    var otherKey = GetPropertyValue(otherKeyProp);

                    if (currentKey != null && otherKey != null && Equals(currentKey, otherKey))
                    {
                        duplicates++;
                    }
                }

                isDuplicate = duplicates > 1;
            }

            if (isDuplicate)
            {
                var bgColor = new Color(1f, 0.5f, 0.5f, 0.3f);
                EditorGUI.DrawRect(position, bgColor);
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        private SerializedProperty FindParentList(SerializedProperty property)
        {
            var path = property.propertyPath;
            var parentPath = path.Substring(0, path.LastIndexOf(".Array.data"));
            return property.serializedObject.FindProperty(parentPath);
        }

        private object GetPropertyValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return property.intValue;
                case SerializedPropertyType.Boolean:
                    return property.boolValue;
                case SerializedPropertyType.Float:
                    return property.floatValue;
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Color:
                    return property.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue;
                case SerializedPropertyType.Enum:
                    return property.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return property.vector2Value;
                case SerializedPropertyType.Vector3:
                    return property.vector3Value;
                case SerializedPropertyType.Vector4:
                    return property.vector4Value;
                case SerializedPropertyType.ExposedReference:
                    return property.exposedReferenceValue;
                default:
                    return null;
            }
        }
    }
}
