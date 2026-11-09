using System;
using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public class VariableTypeSerializable
    {
        [SerializeField] private string typeName = nameof(DialogueStringVariable);

        public Type RuntimeType => Type.GetType($"{nameof(DialogueSystem)}.{typeName}");

        public VariableTypeSerializable(Type type)
        {
            typeName = type.Name;
        }

        public void CopyFrom(VariableTypeSerializable other)
        {
            typeName = other.typeName;
        }
    }
}
