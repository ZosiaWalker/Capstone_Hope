using System.Collections.Generic;

namespace DialogueSystem
{
    public static class MessageManager
    {
        public static string GetMessage(DictionarySerializable<Language, string> messages, Language language)
        {
            if (messages.ContainsKey(language))
            {
                string message = messages[language];
                DialogueVariableManager dialogueVariableManager = DialogueVariableManager.Instance;

                if (dialogueVariableManager != null)
                {
                    message = System.Text.RegularExpressions.Regex.Replace(message, @"\{([SIFB]):(.*?)\}", match =>
                    {
                        string typeCode = match.Groups[1].Value;
                        string variableName = match.Groups[2].Value;

                        switch (typeCode)
                        {
                            case "S":
                                return GetStringValue(variableName, dialogueVariableManager) ?? match.Value;
                            case "I":
                                int? intValue = GetIntValue(variableName, dialogueVariableManager);
                                return intValue != null ? intValue.ToString() : match.Value;
                            case "F":
                                float? floatValue = GetFloatValue(variableName, dialogueVariableManager);
                                return floatValue != null ? floatValue.ToString() : match.Value;
                            case "B":
                                bool? boolValue = GetBoolValue(variableName, dialogueVariableManager);
                                return boolValue != null ? boolValue.ToString() : match.Value;
                            default:
                                return match.Value;
                        }
                    });
                }

                return message;
            }
            return null;
        }

        private static string GetStringValue(string variableName, DialogueVariableManager dialogueVariableManager)
        {
            foreach (KeyValuePair<DialogueStringVariable, ReturnEvent<string>> item in dialogueVariableManager.stringVariables)
            {
                if (item.Key.ToString() == variableName)
                {
                    return item.Value.GetValue();
                }
            }

            return null;
        }

        private static int? GetIntValue(string variableName, DialogueVariableManager dialogueVariableManager)
        {
            foreach (KeyValuePair<DialogueIntVariable, ReturnEvent<int>> item in dialogueVariableManager.intVariables)
            {
                if (item.Key.ToString() == variableName)
                {
                    return item.Value.GetValue();
                }
            }

            return null;
        }

        private static float? GetFloatValue(string variableName, DialogueVariableManager dialogueVariableManager)
        {
            foreach (KeyValuePair<DialogueFloatVariable, ReturnEvent<float>> item in dialogueVariableManager.floatVariables)
            {
                if (item.Key.ToString() == variableName)
                {
                    return item.Value.GetValue();
                }
            }

            return null;
        }

        private static bool? GetBoolValue(string variableName, DialogueVariableManager dialogueVariableManager)
        {
            foreach (KeyValuePair<DialogueBoolVariable, ReturnEvent<bool>> item in dialogueVariableManager.boolVariables)
            {
                if (item.Key.ToString() == variableName)
                {
                    return item.Value.GetValue();
                }
            }

            return null;
        }
    }
}
