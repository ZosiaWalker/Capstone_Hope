using UnityEngine;

namespace DialogueSystem
{
    public static class Logger
    {
        public static void LogNullFieldWarning(string treeName, string nodeName, string fieldName)
        {
            Debug.LogWarning($"Warning: In the '{treeName}', the {nodeName} has a null '{fieldName}' field, but you're trying to acces its value. Assign a valid {fieldName} to this node!");
        }

        public static void LogNoDefaultChoiceWarning(string treeName, string nodeName)
        {
            string fieldName = "Default Choice";
            Debug.LogWarning($"Warning: In the '{treeName}', the {nodeName} doesn't have a '{fieldName}' assigned, but you're trying to acces its value. Assign a {fieldName} to this node!");
        }

        public static void LogKeyNotFoundInDictionaryWarning(string key, string dictionary)
        {
            Debug.LogWarning($"Warning: The '{key}' was not found in the '{dictionary}'.");
        }

        public static void LogDuplicateKeyInRegistryWarning(string key, string registry)
        {
            Debug.LogWarning($"Warning: The Dialogue Registry '{registry}' contains multiple references of '{key}'. Ensure there are no duplicates in the '{registry}'.");
        }

        public static void LogInvalidChoicePassedWarning(string functionName, string parameterName)
        {
            Debug.LogWarning($"Warning: You didn't pass a valid '{parameterName}' to the '{functionName}' function. Pass a valid '{parameterName}' to this function!");
        }

        public static void LogLanguageNotFoundWarning(string treeName, string nodeName, string language)
        {
            Debug.LogWarning($"Warning: In the '{treeName}', the {nodeName} doesn't have the '{language}' language set. Set a valid {language} message to this node!");
        }

        public static void LogFieldChangedWarning(string name = "Dialogue Settings")
        {
            Debug.LogWarning($"{name} changed. Make sure you regenerate code via 'Tools/Dialogue System/Regenerate Code'.");
        }

        public static void LogNullParameterPassedError(string functionName, string parameterName)
        {
            Debug.LogError($"Error: The '{parameterName}' you passed to '{functionName}' function is null. Pass a valid '{parameterName}' to this function!");
        }

        public static void LogEmptyNodeOutputError(string treeName, string nodeName)
        {
            Debug.LogError($"Error: In the '{treeName}' the {nodeName} has an empty Output port. Connect the Output port with a valid node!");
        }

        public static void LogGameObjectNotFoundInSceneError(string gameObject)
        {
            Debug.LogError($"Error: The '{gameObject}' was not found in the scene. Add the '{gameObject}' prefab to the scene and make sure that it's enabled!");
        }

        public static void LogNextNodeCalledBeforeStartDialogueError()
        {
            Debug.LogError($"Error: You tried to call 'NextNode' function before calling 'StartDialogue'. You can call 'NextNode' function only when dialogueManager.DialogueStarted == true!");
        }
    }
}
