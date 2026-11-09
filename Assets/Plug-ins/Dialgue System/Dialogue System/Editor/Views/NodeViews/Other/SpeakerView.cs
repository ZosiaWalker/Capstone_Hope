using System.Linq;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class SpeakerView : VisualElement
    {
        public CharacterInternal character;
        private DropdownField speakerField;

        private DialogueSettings dialogueSettings;

        public SpeakerView(CharacterInternal character)
        {
            this.character = character;
            this.dialogueSettings = DialogueSettings.GetOrCreateSettings();

            LoadUIElements();
        }

        private void LoadUIElements()
        {
            speakerField = new DropdownField("Speaker", dialogueSettings.characters, 0);
            speakerField.value = character.characterName;
            speakerField.RegisterValueChangedCallback(evt =>
            {
                character.OnCharacterChanged(evt.newValue);
            });
            Add(speakerField);

            CustomFieldView customFieldView = new CustomFieldView(dialogueSettings.characterFields.ToHashSet(), character.customFieldSO);
            Add(customFieldView);
        }
    }
}
