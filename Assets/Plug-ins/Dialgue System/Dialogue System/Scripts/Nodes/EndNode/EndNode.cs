namespace DialogueSystem
{
    public partial interface EndNode : Node
    {
        public DialogueTreeSO NextDialogue { get; }
    }
}
