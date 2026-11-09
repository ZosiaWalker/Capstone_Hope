using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogueSystem
{
    public partial class DialogueNodeInternal : NodeInternal, DialogueNode
    {
        public CharacterInternal speaker;
        public DictionarySerializable<Language, string> messages = new DictionarySerializable<Language, string>();
        public List<CharacterInternal> listeners = new List<CharacterInternal>();

        public NodeInternal child;

#if UNITY_EDITOR

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
                this.messages[dialogueSettings.language] = message;
                Save();
            }
        }

        public void OnMessageChanged(Language language, string message)
        {
            messages[language] = message;
            Save();
        }

        public void OnChildChanged(NodeInternal child)
        {
            this.child = child;
            Save();
        }

        public override void CopyFrom(NodeInternal other)
        {
            base.CopyFrom(other);
            DialogueNodeInternal otherDialogueNode = other as DialogueNodeInternal;
            if (otherDialogueNode != null)
            {
                OnSpeakerCreated(CharacterInternal.CreateInstance(tree, this));
                this.speaker.CopyFrom(otherDialogueNode.speaker);

                DictionarySerializable<Language, string> messages = new DictionarySerializable<Language, string>();
                foreach (var pair in otherDialogueNode.messages)
                {
                    messages[pair.Key] = pair.Value;
                }
                this.messages = messages;

                foreach (CharacterInternal listener in otherDialogueNode.listeners)
                {
                    OnListenerAdded(CharacterInternal.CreateInstance(tree, this));
                    listeners.Last().CopyFrom(listener);
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

        Character DialogueNode.Speaker
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
        List<Character> DialogueNode.Listeners
        {
            get { return listeners.Select(listener => (Character)listener).ToList(); }
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
    }
}
