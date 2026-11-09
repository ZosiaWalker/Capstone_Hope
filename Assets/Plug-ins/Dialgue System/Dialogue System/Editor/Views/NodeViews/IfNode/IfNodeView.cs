using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class IfNodeView : NodeView
    {
        public Port input;
        public Dictionary<Port, IfPortView> outputs = new Dictionary<Port, IfPortView>();
        public Port elseOutput;
        public VisualElement parentElement;
        public VisualElement bodyElement;
        public VisualElement conditionPortContainer;
        public DialogueTreeSO tree;
        public IfNodeInternal ifNode;

        private VisualElement inputPortContainer;
        private Button addConditionBtn;
        private VisualElement elseContainer;

        public IfNodeView(NodeInternal node, string uxml, DialogueTreeSO tree) : base(node, uxml)
        {
            this.tree = tree;
        }

        private void InitializeVariables()
        {
            ifNode = node as IfNodeInternal;
        }

        protected override void LoadUIElements()
        {
            InitializeVariables();

            Label label = this.Q<Label>("title-label");
            label.text = "If Node";

            parentElement = this.Q<VisualElement>("parent");
            bodyElement = this.Q<VisualElement>("body");
            inputPortContainer = this.Q<VisualElement>("input");
            conditionPortContainer = this.Q<VisualElement>("condition-container");
            elseContainer = this.Q<VisualElement>("else-container");

            addConditionBtn = this.Q<Button>("add-condition");
            if (addConditionBtn != null)
            {
                addConditionBtn.clickable.clicked += AddCondition;
            }

            CustomFieldView customFieldView = new CustomFieldView(dialogueSettings.ifNodeFields.ToHashSet(), ifNode.customFieldSO);
            bodyElement.Add(customFieldView);
        }

        private void AddCondition()
        {
            IfPortSO ifPort = IfPortSO.CreateInstance(ifNode.tree, ifNode);
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            IfPortView ifPortView = new IfPortView(ifPort, output, this, ifNode.conditions.Count);
            conditionPortContainer.Add(ifPortView);
            ifNode.OnConditionAdded(ifPort);
            outputs[output] = ifPortView;
            if (elseOutput == null)
            {
                CreateElseOutput();
            }
        }

        public void OnConditionRemoved()
        {
            if (ifNode.conditions.Count == 0)
            {
                var edge = elseOutput.connections.FirstOrDefault();
                if (edge != null)
                {
                    edge.input.Disconnect(edge);
                    edge.output.Disconnect(edge);
                    tree.RemoveGraphElement(edge);
                }
                ifNode.OnElseChildChanged(null);
                RemoveElseOutput();
            }
        }

        private void CreateElseOutput()
        {
            elseContainer.style.display = DisplayStyle.Flex;
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            if (output != null)
            {
                output.portName = "Else Output";
                elseContainer.Add(output);
            }
            elseOutput = output;
        }

        private void RemoveElseOutput()
        {
            elseContainer.Remove(elseOutput);
            elseContainer.style.display = DisplayStyle.None;
            elseOutput = null;
        }

        protected override void CreateInputPorts()
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            if (input != null)
            {
                input.portName = "Input";
                inputPortContainer.Add(input);
            }
        }

        protected override void CreateOutputPorts()
        {
            for (int i = 0; i < ifNode.conditions.Count; i++)
            {
                Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                IfPortView ifPortView = new IfPortView(ifNode.conditions[i], output, this, i);
                conditionPortContainer.Add(ifPortView);
                outputs[output] = ifPortView;
                if (i == 0)
                {
                    CreateElseOutput();
                }
            }
        }

        public override void OnSelected()
        {
            base.OnSelected();
            parentElement.AddToClassList("selected");
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            parentElement.RemoveFromClassList("selected");
        }
    }
}
