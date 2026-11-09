using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class ChoiceInternalView
    {
        public ChoiceInternal choice;
        public Port output;
        public Toggle isDefaultChoiceToggle;

        private ChoiceNodeView choiceNodeView;
        private VisualElement parentLayout;
        private Button removeChoiceBtn;
        private TextField textField;
        public int index;
        private string message;

        private DialogueSettings dialogueSettings;

        public ChoiceInternalView(ChoiceInternal choice, ChoiceNodeView choiceNodeView, int index, Port output)
        {
            this.choice = choice;
            this.choiceNodeView = choiceNodeView;
            this.index = index;
            this.message = choice.GetMessageInternal();
            this.output = output;
            this.dialogueSettings = DialogueSettings.GetOrCreateSettings();

            LoadUIElements();
        }

        public void OnIndexChanged(int index)
        {
            this.index = index;
            textField.label = $"Choice {index + 1} Message";
            output.portName = $"Output {index + 1}";
        }

        private void LoadUIElements()
        {
            parentLayout = new VisualElement();
            if (parentLayout != null)
            {
                choiceNodeView.choicePortContainer.Add(parentLayout);
            }

            VisualElement layout = new VisualElement();
            if (layout != null)
            {
                layout.style.flexGrow = 1;
                layout.style.justifyContent = Justify.SpaceBetween;
                layout.style.flexDirection = FlexDirection.Row;
                layout.style.alignContent = Align.Center;
                parentLayout.Add(layout);
            }

            removeChoiceBtn = new Button();
            if (removeChoiceBtn != null)
            {
                removeChoiceBtn.text = "X";
                removeChoiceBtn.clickable.clicked += RemoveChoiceOption;
                layout.Add(removeChoiceBtn);
            }
            if (output != null)
            {
                output.portName = $"Output {index + 1}";
                layout.Add(output);
            }

            isDefaultChoiceToggle = new Toggle();
            if (isDefaultChoiceToggle != null)
            {
                isDefaultChoiceToggle.label = "Default Choice";
                isDefaultChoiceToggle.value = choice.isDefaultChoice;
                isDefaultChoiceToggle.RegisterValueChangedCallback(evt =>
                {
                    choiceNodeView.ResetDefaultChoice(this, evt.newValue);
                });
                parentLayout.Add(isDefaultChoiceToggle);
            }

            textField = new TextField();
            if (textField != null)
            {
                textField.label = $"Choice {index + 1} Message";
                textField.value = $"{message}";
                textField.multiline = true;
                textField.style.whiteSpace = WhiteSpace.Normal;
                textField.RegisterValueChangedCallback(evt =>
                {
                    choice.OnMessageChanged(evt.newValue);
                });
                parentLayout.Add(textField);
            }

            CustomFieldView customFieldView = new CustomFieldView(dialogueSettings.choiceFields.ToHashSet(), choice.customFieldSO);
            parentLayout.Add(customFieldView);
        }

        private void RemoveChoiceOption()
        {
            foreach (var pair in choiceNodeView.outputs)
            {
                ChoiceInternalView choicePortView = pair.Value;
                if (choicePortView.index > index)
                {
                    choicePortView.OnIndexChanged(choicePortView.index - 1);
                }
            }
            choiceNodeView.choiceNode.OnChoiceRemoved(choice);
            choiceNodeView.choicePortContainer.Remove(parentLayout);
            choiceNodeView.outputs.Remove(output);
            var edge = output.connections.FirstOrDefault();
            if (edge != null)
            {
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                choiceNodeView.tree.RemoveGraphElement(edge);
            }
        }
    }
}
