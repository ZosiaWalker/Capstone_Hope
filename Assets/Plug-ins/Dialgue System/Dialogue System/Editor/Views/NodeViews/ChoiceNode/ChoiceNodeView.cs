using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class ChoiceNodeView : NodeView
    {
        public Port input;
        public Dictionary<Port, ChoiceInternalView> outputs = new Dictionary<Port, ChoiceInternalView>();
        public VisualElement choicePortContainer;
        public DialogueTreeSO tree;
        public ChoiceNodeInternal choiceNode;

        private VisualElement parentElement;
        private VisualElement bodyElement;
        private VisualElement inputPortContainer;
        private Button addChoiceBtn;
        private VisualElement listenerContainer;
        private Button addListenerBtn;

        public ChoiceNodeView(NodeInternal node, string uxml, DialogueTreeSO tree) : base(node, uxml)
        {
            this.tree = tree;
        }

        private void InitializeVariables()
        {
            choiceNode = node as ChoiceNodeInternal;
        }

        protected override void LoadUIElements()
        {
            InitializeVariables();

            Label label = this.Q<Label>("title-label");
            label.text = "Choice Node";

            parentElement = this.Q<VisualElement>("parent");
            bodyElement = this.Q<VisualElement>("body");
            VisualElement speakerContainer = this.Q<VisualElement>("speaker-container");
            inputPortContainer = this.Q<VisualElement>("input");
            choicePortContainer = this.Q<VisualElement>("choice-container");

            addChoiceBtn = this.Q<Button>("add-choice");
            if (addChoiceBtn != null)
            {
                addChoiceBtn.clickable.clicked += AddChoiceOption;
            }

            CharacterInternal speaker = choiceNode.speaker;
            if (speaker == null)
            {
                speaker = CharacterInternal.CreateInstance(node.tree, choiceNode);
                choiceNode.OnSpeakerCreated(speaker);
            }
            if (speaker.customFieldSO == null)
            {
                choiceNode.AddCustomFieldSOToCharacter(speaker);
            }
            SpeakerView speakerView = new SpeakerView(speaker);
            speakerContainer.Add(speakerView);

            addListenerBtn = this.Q<Button>("add-listener");
            if (addListenerBtn != null)
            {
                addListenerBtn.clickable.clicked += AddListener;
            }

            listenerContainer = this.Q<VisualElement>("listener-container");
            for (int i = 0; i < choiceNode.listeners.Count; i++)
            {
                CharacterInternal listener = choiceNode.listeners[i];
                if (listener.customFieldSO == null)
                {
                    choiceNode.AddCustomFieldSOToCharacter(listener);
                }
                ListenerView listenerView = new ListenerView(listener, this, i);
                listenerContainer.Add(listenerView);
            }

            TextField message = this.Q<TextField>("message");
            if (message != null)
            {
                message.value = choiceNode.GetMessageInternal();
                message.RegisterValueChangedCallback(evt =>
                {
                    choiceNode.OnMessageChanged(evt.newValue);
                });
            }

            CustomFieldView customFieldView = new CustomFieldView(dialogueSettings.choiceNodeFields.ToHashSet(), choiceNode.customFieldSO);
            bodyElement.Add(customFieldView);
        }

        private void AddListener()
        {
            CharacterInternal listener = CharacterInternal.CreateInstance(node.tree, choiceNode);

            int index = choiceNode.listeners.Count;
            choiceNode.OnListenerAdded(listener);
            ListenerView listenerView = new ListenerView(listener, this, index);
            listenerContainer.Add(listenerView);
        }

        public void RemoveListener(ListenerView listenerView)
        {
            choiceNode.OnListenerRemoved(listenerView.character);
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

        public void ResetDefaultChoice(ChoiceInternalView choicePortView, bool value)
        {
            foreach (var keyValuePair in outputs)
            {
                ChoiceInternalView currentChoicePortView = keyValuePair.Value;
                if (currentChoicePortView == choicePortView)
                {
                    currentChoicePortView.choice.OnIsDefaultChoiceChanged(value);
                }
                else
                {
                    currentChoicePortView.isDefaultChoiceToggle.SetValueWithoutNotify(false);
                    currentChoicePortView.choice.OnIsDefaultChoiceChanged(false);
                }
            }
        }

        private void AddChoiceOption()
        {
            ChoiceInternal choicePort = ChoiceInternal.CreateInstance(choiceNode.tree);
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

            int index = choiceNode.choices.Count;
            choiceNode.OnChoiceAdded(choicePort);
            ChoiceInternalView choicePortView = new ChoiceInternalView(choicePort, this, index, output);
            outputs[output] = choicePortView;
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
            for (int i = 0; i < choiceNode.choices.Count; i++)
            {
                Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                ChoiceInternal choice = choiceNode.choices[i];
                if (choice.customFieldSO == null)
                {
                    choiceNode.AddCustomFieldSOToChoice(choice);
                }
                ChoiceInternalView choicePortView = new ChoiceInternalView(choiceNode.choices[i], this, i, output);
                outputs[output] = choicePortView;
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
