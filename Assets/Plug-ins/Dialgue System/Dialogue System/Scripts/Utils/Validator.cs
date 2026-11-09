using System.Linq;
using System.Text;

namespace DialogueSystem
{
    public static class Validator
    {
        private static readonly string[] keywords = new[]
        {
            "abstract","as","base","bool","break","byte","case","catch","char","checked","class","const",
            "continue","decimal","default","delegate","do","double","else","enum","event","explicit",
            "extern","false","finally","fixed","float","for","foreach","goto","if","implicit","in","int",
            "interface","internal","is","lock","long","namespace","new","null","object","operator","out",
            "override","params","private","protected","public","readonly","ref","return","sbyte","sealed",
            "short","sizeof","stackalloc","static","string","struct","switch","this","throw","true","try",
            "typeof","uint","ulong","unchecked","unsafe","ushort","using","virtual","void","volatile","while",
            "DialogueEvent", "DialogueStringVariable", "DialogueIntVariable", "DialogueFloatVariable", "DialogueBoolVariable",
            "Language"
        };

        public static string ValidateIdentifier(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "None";

            StringBuilder sb = new StringBuilder(input.Length);
            string trimmedInput = input.Trim();
            bool isFirstChar = true;

            foreach (char c in trimmedInput)
            {
                char replacementChar = c;

                if (c == ' ' || c == '-' || c == '.')
                {
                    replacementChar = '_';
                }

                if (char.IsLetterOrDigit(replacementChar) || replacementChar == '_')
                {
                    if (isFirstChar)
                    {
                        if (!char.IsLetter(replacementChar) && replacementChar != '_')
                        {
                            sb.Append('_');
                        }
                        isFirstChar = false;
                    }

                    sb.Append(replacementChar);
                }
            }

            string sanitized = sb.ToString();

            if (string.IsNullOrEmpty(sanitized))
                return "None";

            if (keywords.Contains(sanitized))
                sanitized = "_" + sanitized;

            return sanitized;
        }
    }
}