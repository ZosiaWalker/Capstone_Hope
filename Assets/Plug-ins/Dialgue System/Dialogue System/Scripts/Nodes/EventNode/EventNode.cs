namespace DialogueSystem
{
    public partial interface EventNode : Node
    {
        public DialogueEvent DialogueEvent { get; }
    }
}
