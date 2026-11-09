using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    [CustomPropertyDrawer(typeof(ReturnEvent<>), true)]
    public class ReturnEventDrawer : PropertyDrawer
    {
        private const int NumLines = 3;
        private const float Padding = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;

            Rect gameObjectRect = new Rect(position.x, position.y, position.width, lineHeight);

            Rect componentRect = new Rect(
                position.x,
                position.y + lineHeight + Padding,
                position.width,
                lineHeight);

            Rect fieldRect = new Rect(
                position.x,
                position.y + 2 * (lineHeight + Padding),
                position.width,
                lineHeight);

            SerializedProperty targetGameObjectProp = property.FindPropertyRelative("targetGameObject");
            SerializedProperty componentNameProp = property.FindPropertyRelative("componentName");
            SerializedProperty fieldOrPropertyNameProp = property.FindPropertyRelative("fieldOrPropertyName");

            Type requiredType = GetGenericArgumentType(fieldInfo.FieldType);

            EditorGUI.PropertyField(
                gameObjectRect,
                targetGameObjectProp,
                new GUIContent("Game Object"));

            if (targetGameObjectProp.objectReferenceValue == null)
            {
                EditorGUI.EndProperty();
                return;
            }

            GameObject targetGameObject = (GameObject)targetGameObjectProp.objectReferenceValue;

            Component[] components = targetGameObject.GetComponents<Component>();

            List<string> componentOptions = new List<string>();

            Dictionary<string, Component> componentMap = new Dictionary<string, Component>();

            Dictionary<string, FieldInfo> nestedFieldMap = new Dictionary<string, FieldInfo>();

            foreach (Component component in components)
            {
                if (component == null)
                {
                    continue;
                }

                Type componentType = component.GetType();

                string componentKey = componentType.FullName;
                string componentDisplayName = componentType.Name;

                componentOptions.Add(componentDisplayName);

                if (!componentMap.ContainsKey(componentDisplayName))
                {
                    componentMap.Add(componentDisplayName, component);
                }

                FieldInfo[] fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);

                foreach (FieldInfo field in fields)
                {
                    if (!IsClassOrStruct(field.FieldType))
                    {
                        continue;
                    }

                    string nestedKey = $"{componentKey}.{field.Name}";
                    string nestedDisplayName = $"{componentDisplayName}.{field.Name}";

                    componentOptions.Add(nestedDisplayName);

                    if (!nestedFieldMap.ContainsKey(nestedDisplayName))
                    {
                        nestedFieldMap.Add(nestedDisplayName, field);
                    }
                }
            }

            if (componentOptions.Count == 0)
            {
                EditorGUI.EndProperty();
                return;
            }

            int selectedComponentIndex =
                componentOptions.IndexOf(componentNameProp.stringValue);

            if (selectedComponentIndex < 0)
            {
                selectedComponentIndex = 0;
            }

            selectedComponentIndex = EditorGUI.Popup(
                componentRect,
                "Component",
                selectedComponentIndex,
                componentOptions.ToArray());

            if (selectedComponentIndex < 0 ||
                selectedComponentIndex >= componentOptions.Count)
            {
                EditorGUI.EndProperty();
                return;
            }

            string selectedOption = componentOptions[selectedComponentIndex];

            componentNameProp.stringValue = selectedOption;

            List<string> fieldAndPropertyNames = new List<string>();

            if (nestedFieldMap.TryGetValue(selectedOption, out FieldInfo nestedField))
            {
                GatherFields(
                    nestedField.FieldType,
                    requiredType,
                    fieldAndPropertyNames);
            }
            else if (componentMap.TryGetValue(selectedOption, out Component selectedComponent))
            {
                GatherFieldsAndProperties(
                    selectedComponent.GetType(),
                    requiredType,
                    fieldAndPropertyNames);
            }

            if (fieldAndPropertyNames.Count == 0)
            {
                EditorGUI.HelpBox(
                    fieldRect,
                    "No matching fields or properties found.",
                    MessageType.Info);

                EditorGUI.EndProperty();
                return;
            }

            int selectedFieldIndex = fieldAndPropertyNames.IndexOf(fieldOrPropertyNameProp.stringValue);

            if (selectedFieldIndex < 0)
            {
                selectedFieldIndex = 0;
            }

            selectedFieldIndex = EditorGUI.Popup(
                fieldRect,
                "Field/Property",
                selectedFieldIndex,
                fieldAndPropertyNames.ToArray());

            if (selectedFieldIndex >= 0 &&
                selectedFieldIndex < fieldAndPropertyNames.Count)
            {
                fieldOrPropertyNameProp.stringValue =
                    fieldAndPropertyNames[selectedFieldIndex];
            }

            EditorGUI.EndProperty();
        }

        private Type GetGenericArgumentType(Type returnType)
        {
            if (returnType.IsGenericType &&
                returnType.GetGenericTypeDefinition() == typeof(ReturnEvent<>))
            {
                return returnType.GetGenericArguments()[0];
            }

            return null;
        }

        private void GatherFieldsAndProperties(
            Type type,
            Type requiredType,
            List<string> result)
        {
            if (type == null || requiredType == null)
            {
                return;
            }

            FieldInfo[] fields = type.GetFields(
                BindingFlags.Public | BindingFlags.Instance);

            PropertyInfo[] properties = type.GetProperties(
                BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                if (IsValidType(field.FieldType, requiredType))
                {
                    result.Add(field.Name);
                }
            }

            foreach (PropertyInfo property in properties)
            {
                if (property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                if (!property.CanRead)
                {
                    continue;
                }

                if (IsValidType(property.PropertyType, requiredType))
                {
                    result.Add(property.Name);
                }
            }
        }

        private void GatherFields(
            Type classType,
            Type requiredType,
            List<string> result)
        {
            if (classType == null || requiredType == null)
            {
                return;
            }

            FieldInfo[] fields = classType.GetFields(
                BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                if (IsValidType(field.FieldType, requiredType))
                {
                    result.Add(field.Name);
                }
            }
        }

        private bool IsValidType(Type fieldType, Type requiredType)
        {
            if (fieldType == null || requiredType == null)
            {
                return false;
            }

            return fieldType == requiredType ||
                   requiredType.IsAssignableFrom(fieldType);
        }

        private bool IsClassOrStruct(Type type)
        {
            if (type == null)
            {
                return false;
            }

            return (type.IsClass || (type.IsValueType && !type.IsPrimitive))
                   && type != typeof(string)
                   && type != typeof(int)
                   && type != typeof(float)
                   && type != typeof(bool)
                   && !type.IsEnum;
        }

        public override float GetPropertyHeight(
            SerializedProperty property,
            GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + Padding) * NumLines;
        }
    }
}
