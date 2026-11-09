using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogueSystem
{
    public partial class ChoiceInternal : ScriptableObject, Choice
    {
        public bool isDefaultChoice = false;
        public DictionarySerializable<Language, string> messages = new DictionarySerializable<Language, string>();
        public NodeInternal child;
        public DialogueTreeSO tree;
        public CustomFieldSO customFieldSO;

        public static ChoiceInternal CreateInstance(DialogueTreeSO tree)
        {
            ChoiceInternal choiceInternal = ScriptableObject.CreateInstance<ChoiceInternal>();
            choiceInternal.name = choiceInternal.GetType().ToString();
            choiceInternal.tree = tree;

            return choiceInternal;
        }

#if UNITY_EDITOR
        public void OnIsDefaultChoiceChanged(bool isDefaultChoice)
        {
            this.isDefaultChoice = isDefaultChoice;
            Save();
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

        private void Save()
        {
            if (!EditorUtility.IsDirty(this))
            {
                EditorUtility.SetDirty(this);
            }
        }

        public void CopyFrom(ChoiceInternal other)
        {
            isDefaultChoice = other.isDefaultChoice;
            messages = other.messages.Clone();
            customFieldSO.CopyFrom(other.customFieldSO);
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

        bool Choice.IsDefaultChoice
        {
            get { return isDefaultChoice; }
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
