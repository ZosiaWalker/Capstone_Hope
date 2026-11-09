using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class StartNodeView : NodeView
    {
        public Port output;
        public VisualElement parentElement;
        public VisualElement outputPortContainer;

        public StartNodeView(NodeInternal node, string uxml) : base(node, uxml) { }

        protected override void LoadUIElements()
        {
            Label label = this.Q<Label>("title-label");
            label.text = "Start Node";

            parentElement = this.Q<VisualElement>("parent");
            outputPortContainer = this.Q<VisualElement>("output");
        }

        protected override void CreateInputPorts()
        {
            return;
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
