using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogueSystem
{
    public partial class ChoiceNodeInternal : NodeInternal, ChoiceNode
    {
        public CharacterInternal speaker;
        public DictionarySerializable<Language, string> messages = new DictionarySerializable<Language, string>();
        public List<CharacterInternal> listeners = new List<CharacterInternal>();
        public List<ChoiceInternal> choices = new List<ChoiceInternal>();

#if UNITY_EDITOR
        public void AddCustomFieldSOToChoice(ChoiceInternal choiceInternal)
        {
            CustomFieldSO customFieldSO = CustomFieldSO.CreateInstance();
            choiceInternal.customFieldSO = customFieldSO;
            AssetDatabase.AddObjectToAsset(customFieldSO, this);
            AssetDatabase.SaveAssets();
        }
        public void AddCustomFieldSOToCharacter(CharacterInternal characterInternal)
        {
            CustomFieldSO customFieldSO = CustomFieldSO.CreateInstance();
            characterInternal.customFieldSO = customFieldSO;
            AssetDatabase.AddObjectToAsset(customFieldSO, this);
            AssetDatabase.SaveAssets();
        }

        public void OnSpeakerCreated(CharacterInternal speaker)
        {
            AddCustomFieldSOToCharacter(speaker);

            this.speaker = speaker;
            AssetDatabase.AddObjectToAsset(this.speaker, this);
            AssetDatabase.SaveAssets();
        }

        public void OnListenerAdded(CharacterInternal listener)
        {
            AddCustomFieldSOToCharacter(listener);

            listeners.Add(listener);
            AssetDatabase.AddObjectToAsset(listener, this);
            AssetDatabase.SaveAssets();
        }

        public void OnListenerRemoved(CharacterInternal listener)
        {
            if (listener.customFieldSO != null)
            {
                AssetDatabase.RemoveObjectFromAsset(listener.customFieldSO);
            }
            listeners.Remove(listener);
            AssetDatabase.RemoveObjectFromAsset(listener);
            AssetDatabase.SaveAssets();
        }

        public void OnMessageChanged(string message)
        {
            DialogueSettings dialogueSettings = DialogueSettings.GetOrCreateSettings();
            if (dialogueSettings != null)
            {
                messages[dialogueSettings.language] = message;
                Save();
            }
        }

        public void OnMessageChanged(Language language, string message)
        {
            messages[language] = message;
            Save();
        }

        public void OnChoiceAdded(ChoiceInternal choice)
        {
            AddCustomFieldSOToChoice(choice);

            choices.Add(choice);
            AssetDatabase.AddObjectToAsset(choice, this);
            AssetDatabase.SaveAssets();
        }

        public void OnChoiceRemoved(ChoiceInternal choice)
        {
            if (choice.customFieldSO != null)
            {
                AssetDatabase.RemoveObjectFromAsset(choice.customFieldSO);
            }
            choices.Remove(choice);
            AssetDatabase.RemoveObjectFromAsset(choice);
            AssetDatabase.SaveAssets();
        }

        public override void CopyFrom(NodeInternal other)
        {
            base.CopyFrom(other);
            ChoiceNodeInternal otherChoiceNode = other as ChoiceNodeInternal;
            if (otherChoiceNode != null)
            {
                OnSpeakerCreated(CharacterInternal.CreateInstance(tree, this));
                this.speaker.CopyFrom(otherChoiceNode.speaker);

                DictionarySerializable<Language, string> messages = new DictionarySerializable<Language, string>();
                foreach (var pair in otherChoiceNode.messages)
                {
                    messages[pair.Key] = pair.Value;
                }
                this.messages = messages;

                foreach (CharacterInternal listener in otherChoiceNode.listeners)
                {
                    OnListenerAdded(CharacterInternal.CreateInstance(tree, this));
                    listeners.Last().CopyFrom(listener);
                }

                foreach (ChoiceInternal choice in otherChoiceNode.choices)
                {
                    OnChoiceAdded(ChoiceInternal.CreateInstance(tree));
                    choices.Last().CopyFrom(choice);
                }
            }
        }

        public override void Remove()
        {
            if (speaker.customFieldSO != null)
            {
                AssetDatabase.RemoveObjectFromAsset(speaker.customFieldSO);
            }
            AssetDatabase.RemoveObjectFromAsset(speaker);
            foreach (CharacterInternal listener in listeners)
            {
                if (listener.customFieldSO != null)
                {
                    AssetDatabase.RemoveObjectFromAsset(listener.customFieldSO);
                }
                AssetDatabase.RemoveObjectFromAsset(listener);
            }
            foreach (ChoiceInternal choice in choices)
            {
                if (choice.customFieldSO != null)
                {
                    AssetDatabase.RemoveObjectFromAsset(choice.customFieldSO);
                }
                AssetDatabase.RemoveObjectFromAsset(choice);
            }
        }
#endif

        public string GetMessageInternal()
        {
            DialogueSettings dialogueSettings = DialogueSettings.GetOrCreateSettings();
            if (dialogueSettings != null)
            {
                if (messages.ContainsKey(dialogueSettings.language))
                {
                    return messages[dialogueSettings.language];
                }
            }
            return "";
        }

        public string GetMessageInternal(Language language)
        {
            if (messages.ContainsKey(language))
            {
                return messages[language];
            }
            return "";
        }

        public int GetDefaultChoice()
        {
            return choices.FindIndex(choicePort => choicePort.isDefaultChoice);
        }

        Character ChoiceNode.Speaker
        {
            get
            {
                //if (speaker == null && tree != null)
                //{
                //    string fieldName = "Speaker";
                //    Logger.LogNullFieldWarning(tree.name, name, fieldName);
                //    return null;
                //}
                return speaker;
            }
        }

        public string GetMessage(Language language)
        {
            string message = MessageManager.GetMessage(messages, language);

            if (message == null)
            {
                Logger.LogLanguageNotFoundWarning(tree.name, this.name, language.ToString());
                return "";
            }
            return message;
        }

        List<Choice> ChoiceNode.Choices
        {
            get { return choices.Select(choicePort => (Choice)choicePort).ToList(); }
        }

        int ChoiceNode.DefaultChoice
        {
            get
            {
                int index = choices.FindIndex(choicePort => choicePort.isDefaultChoice);
                if (index == -1 && tree != null)
                {
                    Logger.LogNoDefaultChoiceWarning(tree.name, name);
                }
                return index;
            }
        }

        List<Character> ChoiceNode.Listeners
        {
            get { return listeners.Select(listener => (Character)listener).ToList(); }
        }
    }
}
