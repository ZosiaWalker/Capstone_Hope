using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class EventNodeView : NodeView
    {
        public Port input;
        public Port output;
        public VisualElement parentElement;
        public VisualElement bodyElement;
        public VisualElement inputPortContainer;
        public VisualElement outputPortContainer;

        private EventNodeInternal eventNode;

        public EventNodeView(NodeInternal node, string uxml) : base(node, uxml) { }

        private void InitializeVariables()
        {
            eventNode = node as EventNodeInternal;
        }

        protected override void LoadUIElements()
        {
            InitializeVariables();

            Label label = this.Q<Label>("title-label");
            label.text = "Event Node";

            parentElement = this.Q<VisualElement>("parent");
            bodyElement = this.Q<VisualElement>("body");
            inputPortContainer = this.Q<VisualElement>("input");
            outputPortContainer = this.Q<VisualElement>("output");

            EnumField events = this.Q<EnumField>("event");
            events.value = eventNode.dialogueEvent;
            events.RegisterValueChangedCallback(evt =>
            {
                eventNode.OnDialogueEventChanged((DialogueEvent)evt.newValue);
            });

            CustomFieldView customFieldView = new CustomFieldView(dialogueSettings.eventNodeFields.ToHashSet(), eventNode.customFieldSO);
            bodyElement.Add(customFieldView);
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
            output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            if (output != null)
            {
                output.portName = "Output";
                outputPortContainer.Add(output);
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
