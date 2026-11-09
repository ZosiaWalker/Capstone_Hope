namespace DialogueSystem
{
    public partial interface Choice
    {
        public bool IsDefaultChoice { get; }

        public string GetMessage(Language language);
    }
}
