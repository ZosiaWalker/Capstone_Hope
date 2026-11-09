using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class EndNodeView : NodeView
    {
        public Port input;
        public VisualElement parentElement;
        public VisualElement bodyElement;
        public VisualElement inputPortContainer;

        public EndNodeView(NodeInternal node, string uxml) : base(node, uxml) { }

        protected override void LoadUIElements()
        {
            EndNodeInternal endNode = node as EndNodeInternal;

            Label label = this.Q<Label>("title-label");
            label.text = "End Node";

            parentElement = this.Q<VisualElement>("parent");
            bodyElement = this.Q<VisualElement>("body");
            inputPortContainer = this.Q<VisualElement>("input");

            ObjectField nextDialogue = this.Q<ObjectField>("next-dialogue");
            nextDialogue.value = endNode.nextDialogue;
            nextDialogue.RegisterValueChangedCallback(evt =>
            {
                endNode.OnNextDialogueChanged(evt.newValue as DialogueTreeSO);
            });

            CustomFieldView customFieldView = new CustomFieldView(dialogueSettings.endNodeFields.ToHashSet(), endNode.customFieldSO);
            bodyElement.Add(customFieldView);
        }

        protected override void CreateInputPorts()
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            if (input != null)
            {
                input.portName = "Input";
                inputContainer.Add(input);
            }
        }

        protected override void CreateOutputPorts()
        {
            return;
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
