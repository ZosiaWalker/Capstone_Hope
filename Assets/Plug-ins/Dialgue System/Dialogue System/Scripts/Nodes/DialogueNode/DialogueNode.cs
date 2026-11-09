using System.Collections.Generic;

namespace DialogueSystem
{
    public partial interface DialogueNode : Node
    {
        public Character Speaker { get; }

        public List<Character> Listeners { get; }

        public string GetMessage(Language language);
    }
}
