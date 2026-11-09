using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class IfPortView : VisualElement
    {
        public IfPortSO ifPort;
        public Port output;

        private IfNodeView ifNodeView;
        public int index;

        private EnumField variables;
        private Button removeConditionBtn;
        private VisualElement variableView;

        public IfPortView(IfPortSO ifPort, Port output, IfNodeView ifNodeView, int index)
        {
            this.ifPort = ifPort;
            this.output = output;
            this.ifNodeView = ifNodeView;
            this.index = index;

            LoadUIElements();
        }

        private void LoadUIElements()
        {
            VisualElement layout = new VisualElement();
            if (layout != null)
            {
                layout.style.flexGrow = 1;
                layout.style.justifyContent = Justify.SpaceBetween;
                layout.style.flexDirection = FlexDirection.Row;
                layout.style.alignContent = Align.Center;
                Add(layout);
            }

            removeConditionBtn = new Button();
            if (removeConditionBtn != null)
            {
                removeConditionBtn.text = "X";
                removeConditionBtn.clickable.clicked += RemoveCondition;
                layout.Add(removeConditionBtn);
            }
            if (output != null)
            {
                output.portName = $"Condition {index + 1} Output";
                layout.Add(output);
            }

            List<Type> enumTypes = new List<Type>
            {
                typeof(DialogueStringVariable),
                typeof(DialogueIntVariable),
                typeof(DialogueFloatVariable),
                typeof(DialogueBoolVariable)
            };
            var popup = new PopupField<Type>(
                enumTypes,
                0,
                t => t.Name.Replace("Dialogue", "").Replace("Variable", ""),
                t => t.Name.Replace("Dialogue", "").Replace("Variable", "")
            );
            popup.label = "Variable Type";
            if (ifPort.variableType != null)
            {
                popup.value = ifPort.variableType.RuntimeType;
            }
            popup.RegisterValueChangedCallback(evt =>
            {
                OnVariableTypeChanged(evt.newValue);
                ifPort.OnVariableTypeChanged(evt.newValue);
                InitializeVariables(evt.newValue);
            });
            Add(popup);

            variables = new EnumField();
            variables.label = (index == 0) ? "If" : "Else if";
            InitializeVariables(ifPort.variableType.RuntimeType);
            variables.RegisterValueChangedCallback(evt =>
            {
                int value = Convert.ToInt32(evt.newValue);
                ifPort.OnVariableChanged(value);
            });
            Add(variables);
            LoadFields(ifPort.variableType.RuntimeType);
        }

        private void InitializeVariables(Type variableType)
        {
            if (ifPort.variableType.RuntimeType == typeof(DialogueStringVariable))
            {
                variables.Init(ifPort.stringVariable);
            }
            else if (ifPort.variableType.RuntimeType == typeof(DialogueIntVariable))
            {
                variables.Init(ifPort.intVariable);
            }
            else if (ifPort.variableType.RuntimeType == typeof(DialogueFloatVariable))
            {
                variables.Init(ifPort.floatVariable);
            }
            else if (ifPort.variableType.RuntimeType == typeof(DialogueBoolVariable))
            {
                variables.Init(ifPort.boolVariable);
            }
        }

        public void OnIndexChanged(int index)
        {
            this.index = index;
            output.portName = $"Condition {index + 1} Output";
            variables.label = (index == 0) ? "If" : "Else if";
        }

        private void OnVariableTypeChanged(Type variableType)
        {
            if (Contains(variableView))
            {
                Remove(variableView);
                ifPort.OnOperatorChanged(null);
                ifPort.OnValueChanged(null);
            }
            LoadFields(variableType);
        }

        private void LoadFields(Type variableType)
        {
            if (variableType == typeof(DialogueStringVariable))
            {
                variableView = new IfPortStringVariableView(ifPort);
                Add(variableView);
            }
            else if (variableType == typeof(DialogueIntVariable))
            {
                variableView = new IfPortIntVariableView(ifPort);
                Add(variableView);
            }
            else if (variableType == typeof(DialogueFloatVariable))
            {
                variableView = new IfPortFloatVariableView(ifPort);
                Add(variableView);
            }
            else if (variableType == typeof(DialogueBoolVariable))
            {
                variableView = new IfPortBoolVariableView(ifPort);
                Add(variableView);
            }
        }

        private void RemoveCondition()
        {
            foreach (var pair in ifNodeView.outputs)
            {
                IfPortView ifPortView = pair.Value;
                if (ifPortView.index > index)
                {
                    ifPortView.OnIndexChanged(ifPortView.index - 1);
                }
            }
            ifNodeView.ifNode.OnConditionRemoved(ifPort);
            ifNodeView.OnConditionRemoved();
            ifNodeView.conditionPortContainer.Remove(this);
            ifNodeView.outputs.Remove(output);
            var edge = output.connections.FirstOrDefault();
            if (edge != null)
            {
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                ifNodeView.tree.RemoveGraphElement(edge);
            }
        }
    }
}
