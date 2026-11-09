using System.Collections.Generic;

namespace DialogueSystem
{
    public partial interface ChoiceNode : Node
    {
        public Character Speaker { get; }

        public List<Choice> Choices { get; }

        public int DefaultChoice { get; }

        public List<Character> Listeners { get; }

        public string GetMessage(Language language);
    }
}
