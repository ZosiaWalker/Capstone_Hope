using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue Registry", menuName = "Dialogue System/Dialogue Registry", order = -2)]
    public class DialogueRegistrySO : ScriptableObject
    {
        [SerializeField]
        private List<DialogueTreeSO> dialogues;

        private Dictionary<string, DialogueTreeSO> dialoguesDictionary;

        public void Initialize()
        {
            if (dialoguesDictionary != null) return;

            dialoguesDictionary = new Dictionary<string, DialogueTreeSO>();

            foreach (var dialogue in dialogues)
            {
                if (dialogue == null || string.IsNullOrEmpty(dialogue.DialogueID))
                {
                    continue;
                }

                if (dialoguesDictionary.ContainsKey(dialogue.DialogueID))
                {
                    Logger.LogDuplicateKeyInRegistryWarning(dialogue.name, name);
                }
                else
                {
                    dialoguesDictionary.Add(dialogue.DialogueID, dialogue);
                }
            }
        }

        public DialogueTreeSO GetDialogueByID(string id)
        {
            Initialize();
            dialoguesDictionary.TryGetValue(id, out var result);
            return result;
        }

        public bool ContainsID(string id)
        {
            Initialize();
            return dialoguesDictionary.ContainsKey(id);
        }
    }
}
