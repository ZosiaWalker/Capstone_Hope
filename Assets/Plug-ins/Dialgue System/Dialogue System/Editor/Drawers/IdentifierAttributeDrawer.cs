using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    [CustomPropertyDrawer(typeof(IdentifierAttribute))]
    public class IdentifierAttributeDrawer : PropertyDrawer
    {
        bool hasCheckedGUID = false;

        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(rect, prop, label);
            GUI.enabled = true;

            if (!hasCheckedGUID)
                FetchGUIDFromFile(prop);
        }

        void FetchGUIDFromFile(SerializedProperty prop)
        {
            if (prop.propertyType != SerializedPropertyType.String)
                return;

            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(
                prop.serializedObject.targetObject,
                out string guid,
                out long localID))
            {
                if (prop.stringValue != guid)
                {
                    prop.stringValue = guid;
                    prop.serializedObject.ApplyModifiedProperties();
                }
            }
            hasCheckedGUID = true;
        }
    }
}
