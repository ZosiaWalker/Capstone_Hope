using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogueSystem
{
    public class IfPortSO : ScriptableObject
    {
        public VariableTypeSerializable variableType = new VariableTypeSerializable(typeof(DialogueStringVariable));
        public DialogueStringVariable stringVariable;
        public DialogueIntVariable intVariable;
        public DialogueFloatVariable floatVariable;
        public DialogueBoolVariable boolVariable;

        public string operatorValue;
        public string value;

        public NodeInternal child;

        [HideInInspector]
        public DialogueTreeSO tree;

        [HideInInspector]
        public NodeInternal node;

        public static IfPortSO CreateInstance(DialogueTreeSO tree, NodeInternal node)
        {
            IfPortSO ifPort = ScriptableObject.CreateInstance<IfPortSO>();
            ifPort.name = ifPort.GetType().ToString();
            ifPort.tree = tree;
            ifPort.node = node;

            return ifPort;
        }

#if UNITY_EDITOR

        public void OnVariableTypeChanged(Type variableType)
        {
            this.variableType = new VariableTypeSerializable(variableType);
            Save();
        }

        public void OnVariableChanged(int index)
        {
            if (variableType.RuntimeType == typeof(DialogueStringVariable))
            {
                stringVariable = (DialogueStringVariable)index;
            }
            else if (variableType.RuntimeType == typeof(DialogueIntVariable))
            {
                intVariable = (DialogueIntVariable)index;
            }
            else if (variableType.RuntimeType == typeof(DialogueFloatVariable))
            {
                floatVariable = (DialogueFloatVariable)index;
            }
            else if (variableType.RuntimeType == typeof(DialogueBoolVariable))
            {
                boolVariable = (DialogueBoolVariable)index;
            }
        }

        public void OnOperatorChanged(string operatorValue)
        {
            this.operatorValue = operatorValue;
            Save();
        }

        public void OnValueChanged(string value)
        {
            this.value = value;
            Save();
        }

        public void OnChildChanged(NodeInternal child)
        {
            this.child = child;
            Save();
        }

        private void Save()
        {
            if (!EditorUtility.IsDirty(this))
            {
                EditorUtility.SetDirty(this);
            }
        }

        public void CopyFrom(IfPortSO other)
        {
            variableType.CopyFrom(other.variableType);
            stringVariable = other.stringVariable;
            intVariable = other.intVariable;
            floatVariable = other.floatVariable;
            boolVariable = other.boolVariable;
            operatorValue = other.operatorValue;
            value = other.value;
        }
#endif

        public bool GetCondition(string value)
        {
            if (ContainsNullField())
            {
                return false;
            }
            if (operatorValue == "==")
            {
                return value == this.value;
            }
            else if (operatorValue == "!=")
            {
                return value != this.value;
            }
            return false;
        }

        public bool GetCondition(int value)
        {
            if (ContainsNullField())
            {
                return false;
            }
            int intValue;
            if (!int.TryParse(this.value, out intValue))
            {
                Debug.LogWarning("Couldn't parse value to int.");
                return false;
            }
            if (operatorValue == "==")
            {
                return value == intValue;
            }
            else if (operatorValue == "!=")
            {
                return value != intValue;
            }
            else if (operatorValue == "<")
            {
                return value < intValue;
            }
            else if (operatorValue == "<=")
            {
                return value <= intValue;
            }
            else if (operatorValue == ">")
            {
                return value > intValue;
            }
            else if (operatorValue == ">=")
            {
                return value >= intValue;
            }
            return false;
        }

        public bool GetCondition(float value)
        {
            if (ContainsNullField())
            {
                return false;
            }
            float floatValue;
            if (!float.TryParse(this.value, out floatValue))
            {
                Debug.LogWarning("Couldn't parse value to float.");
                return false;
            }
            if (operatorValue == "==")
            {
                return value == floatValue;
            }
            else if (operatorValue == "!=")
            {
                return value != floatValue;
            }
            else if (operatorValue == "<")
            {
                return value < floatValue;
            }
            else if (operatorValue == "<=")
            {
                return value <= floatValue;
            }
            else if (operatorValue == ">")
            {
                return value > floatValue;
            }
            else if (operatorValue == ">=")
            {
                return value >= floatValue;
            }
            return false;
        }

        public bool GetCondition(bool value)
        {
            if (ContainsNullField())
            {
                return false;
            }
            bool boolValue;
            if (!bool.TryParse(this.value, out boolValue))
            {
                Debug.LogWarning("Couldn't parse value to bool.");
                return false;
            }
            if (operatorValue == "==")
            {
                return value == boolValue;
            }
            else if (operatorValue == "!=")
            {
                return value != boolValue;
            }
            return false;
        }

        public bool ContainsNullField()
        {
            //if (variable == null)
            //{
            //    string fieldName = "If/Else if";
            //    Logger.LogNullFieldWarning(tree.name, node.name, fieldName);
            //    return true;
            //}
            if (operatorValue == "")
            {
                string fieldName = "Operator";
                Logger.LogNullFieldWarning(tree.name, node.name, fieldName);
                return true;
            }
            if (value == "")
            {
                string fieldName = "Value";
                Logger.LogNullFieldWarning(tree.name, node.name, fieldName);
                return true;
            }
            return false;
        }
    }
}
