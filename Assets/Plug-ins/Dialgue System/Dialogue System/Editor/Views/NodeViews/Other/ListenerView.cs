using System.Linq;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class ListenerView : VisualElement
    {
        public CharacterInternal character;
        private NodeView nodeView;
        public int index;

        private Button removeListenerBtn;
        private DropdownField listenerField;

        private DialogueSettings dialogueSettings;

        public ListenerView(CharacterInternal listener, NodeView nodeView, int index)
        {
            this.character = listener;
            this.nodeView = nodeView;
            this.index = index;
            this.dialogueSettings = DialogueSettings.GetOrCreateSettings();

            LoadUIElements();
        }

        private void LoadUIElements()
        {
            removeListenerBtn = new Button();
            if (removeListenerBtn != null)
            {
                removeListenerBtn.text = "X";
                removeListenerBtn.style.alignSelf = Align.FlexStart;
                removeListenerBtn.clickable.clicked += RemoveListener;
                Add(removeListenerBtn);
            }

            listenerField = new DropdownField($"Listener {index + 1}", dialogueSettings.characters, 0);
            listenerField.value = character.characterName;
            listenerField.RegisterValueChangedCallback(evt =>
            {
                character.OnCharacterChanged(evt.newValue);
            });
            Add(listenerField);

            CustomFieldView customFieldView = new CustomFieldView(dialogueSettings.characterFields.ToHashSet(), character.customFieldSO);
            Add(customFieldView);
        }

        public void OnIndexChanged(int index)
        {
            this.index = index;
            listenerField.label = $"Listener {index + 1}";
        }

        private void RemoveListener()
        {
            if (nodeView is DialogueNodeView)
            {
                ((DialogueNodeView)nodeView).RemoveListener(this);
            }
            else if (nodeView is ChoiceNodeView)
            {
                ((ChoiceNodeView)nodeView).RemoveListener(this);
            }
        }
    }
}
