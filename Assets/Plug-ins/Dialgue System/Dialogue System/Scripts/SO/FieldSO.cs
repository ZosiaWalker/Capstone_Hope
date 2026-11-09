using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "New Field", menuName = "Dialogue System/Field")]
    public class FieldSO : ScriptableObject
    {
        [Identifier]
        [SerializeField]
        private string fieldID;
        public string FieldID
        {
            get
            {
                return fieldID;
            }
        }

        [Delayed] public string label;
        public CustomFieldType fieldType;

        [Highlightable] public List<string> enumChoices = new();

#if UNITY_EDITOR

        private void OnEnable()
        {
            if (label == null)
            {
                label = name;
            }
        }

        private void OnValidate()
        {
            label = Validator.ValidateIdentifier(label);
            for (int i = 0; i < enumChoices.Count; i++)
            {
                enumChoices[i] = Validator.ValidateIdentifier(enumChoices[i]);
            }
        }
#endif
    }
}
