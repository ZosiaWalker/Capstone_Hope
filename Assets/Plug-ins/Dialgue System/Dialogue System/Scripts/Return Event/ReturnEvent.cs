using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DialogueSystem
{

    [Serializable]
    public class ReturnEvent<T>
    {
        public GameObject targetGameObject;
        public string componentName;
        public string fieldOrPropertyName;

        public T GetValue()
        {
            if (targetGameObject == null || string.IsNullOrEmpty(componentName) || string.IsNullOrEmpty(fieldOrPropertyName))
            {
                Debug.LogWarning("ReturnEvent: Missing targetGameObject, componentName, or fieldOrPropertyName.");
                return default;
            }

            List<string> fullPath = componentName.Split('.').ToList();
            fullPath.Add(fieldOrPropertyName);

            object currentObject = targetGameObject.GetComponent(fullPath[0]);

            if (currentObject == null)
            {
                Debug.LogWarning($"ReturnEvent: Component {componentName} not found on {targetGameObject.name}.");
                return default;
            }
            Type currentType = currentObject.GetType();

            foreach (string name in fullPath.Skip(1))
            {
                if (currentObject == null)
                {
                    Debug.LogWarning($"ReturnEvent: Null encountered when accessing {name}.");
                    return default;
                }

                FieldInfo fieldInfo = currentObject.GetType().GetField(name);
                PropertyInfo propertyInfo = currentType.GetProperty(name);

                if (fieldInfo != null)
                {
                    currentObject = fieldInfo.GetValue(currentObject);
                }
                else if (propertyInfo != null)
                {
                    currentObject = propertyInfo.GetValue(currentObject);
                }
                else
                {
                    Debug.LogWarning($"ReturnEvent: Field or property {name} not found in {currentType.Name}.");
                    return default;
                }

                currentType = currentObject?.GetType();
            }
            return (T)currentObject;
        }
    }
}
