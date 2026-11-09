using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueNodeView : NodeView
    {
        public Port input;
        public Port output;
        public VisualElement parentElement;
        public VisualElement bodyElement;
        public VisualElement inputPortContainer;
        public VisualElement outputPortContainer;

        private DialogueNodeInternal dialogueNode;
        private VisualElement listenerContainer;
        private Button addListenerBtn;

        public DialogueNodeView(NodeInternal node, string uxml) : base(node, uxml) { }

        private void InitializeVariables()
        {
            dialogueNode = node as DialogueNodeInternal;
        }

        protected override void LoadUIElements()
        {
            InitializeVariables();

            Label label = this.Q<Label>("title-label");
            label.text = "Dialogue Node";

            parentElement = this.Q<VisualElement>("parent");
            bodyElement = this.Q<VisualElement>("body");
            VisualElement speakerContainer = this.Q<VisualElement>("speaker-container");
            inputPortContainer = this.Q<VisualElement>("input");
            outputPortContainer = this.Q<VisualElement>("output");

            CharacterInternal speaker = dialogueNode.speaker;
            if (speaker == null)
            {
                speaker = CharacterInternal.CreateInstance(node.tree, dialogueNode);
                dialogueNode.OnSpeakerCreated(speaker);
            }
            if (speaker.customFieldSO == null)
            {
                dialogueNode.AddCustomFieldSOToCharacter(speaker);
            }
            SpeakerView speakerView = new SpeakerView(speaker);
            speakerContainer.Add(speakerView);

            addListenerBtn = this.Q<Button>("add-listener");
            if (addListenerBtn != null)
            {
                addListenerBtn.clickable.clicked += AddListener;
            }

            listenerContainer = this.Q<VisualElement>("listener-container");
            for (int i = 0; i < dialogueNode.listeners.Count; i++)
            {
                CharacterInternal listener = dialogueNode.listeners[i];
                if (listener.customFieldSO == null)
                {
                    dialogueNode.AddCustomFieldSOToCharacter(listener);
                }
                ListenerView listenerView = new ListenerView(listener, this, i);
                listenerContainer.Add(listenerView);
            }

            TextField message = this.Q<TextField>("message");
            if (message != null)
            {
                message.value = dialogueNode.GetMessageInternal();
                message.RegisterValueChangedCallback(evt =>
                {
                    dialogueNode.OnMessageChanged(evt.newValue);
                });
            }

            CustomFieldView customFieldView = new CustomFieldView(dialogueSettings.dialogueNodeFields.ToHashSet(), dialogueNode.customFieldSO);
            bodyElement.Add(customFieldView);
        }

        private void AddListener()
        {
            CharacterInternal listener = CharacterInternal.CreateInstance(node.tree, dialogueNode);

            int index = dialogueNode.listeners.Count;
            dialogueNode.OnListenerAdded(listener);
            ListenerView listenerView = new ListenerView(listener, this, index);
            listenerContainer.Add(listenerView);
        }

        public void RemoveListener(ListenerView listenerView)
        {
            dialogueNode.OnListenerRemoved(listenerView.character);
            listenerContainer.Remove(listenerView);
            foreach (VisualElement visualElement in listenerContainer.Children())
            {
                ListenerView listenerViewElement = visualElement as ListenerView;
                if (listenerViewElement != null && listenerViewElement.index > listenerView.index)
                {
                    listenerViewElement.OnIndexChanged(listenerViewElement.index - 1);
                }
            }
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
