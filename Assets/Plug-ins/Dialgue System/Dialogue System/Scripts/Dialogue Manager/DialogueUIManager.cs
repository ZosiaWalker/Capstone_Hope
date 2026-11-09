using UnityEngine;

namespace DialogueSystem
{
    public abstract class DialogueUIManager : MonoBehaviour
    {
        private DialogueManagerInternal dialogueManagerInternal;

        public DialogueManager dialogueManager
        {
            get
            {
                return dialogueManagerInternal;
            }
        }

        public DialogueUIManager()
        {
            this.dialogueManagerInternal = new DialogueManagerInternal(this);
        }

        public abstract void OnDialogueNode(DialogueNode dialogueNode);
        public abstract void OnChoiceNode(ChoiceNode choiceNode);
        public abstract void OnEventNode(EventNode eventNode, DialogueEventManager dialogueEventManager);
        public abstract void OnIfNode(IfNode ifNode);
        public abstract void OnEndNode(EndNode endNode);
    }
}
