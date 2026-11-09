namespace DialogueSystem
{
    public interface DialogueManager
    {
        public Node CurrentNode { get; }

        public bool DialogueStarted { get; }

        public void StartDialogue(DialogueTreeSO dialogueTree);

        public void StartDialogue(DialogueTreeSO dialogueTree, string guid);

        public void NextNode(int choice = -1);

        public void ExitDialogue();
    }
}
