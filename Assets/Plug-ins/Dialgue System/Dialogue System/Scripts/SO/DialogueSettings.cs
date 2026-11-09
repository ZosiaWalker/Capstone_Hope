using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace DialogueSystem
{
    public class DialogueSettings : ScriptableObject
    {
        [HideInInspector] public DialogueTreeSO dialogueTree;

        [HideInInspector] public List<string> characters = new List<string>();
        [HideInInspector][Highlightable] public List<FieldSO> characterFields = new List<FieldSO>();

        [HideInInspector][Highlightable] public List<string> dialogueEvents = new List<string>();

        [HideInInspector][Highlightable] public List<string> dialogueStringVariables = new List<string>();
        [HideInInspector][Highlightable] public List<string> dialogueIntVariables = new List<string>();
        [HideInInspector][Highlightable] public List<string> dialogueFloatVariables = new List<string>();
        [HideInInspector][Highlightable] public List<string> dialogueBoolVariables = new List<string>();

        [HideInInspector][Highlightable] public List<FieldSO> dialogueNodeFields = new List<FieldSO>();
        [HideInInspector][Highlightable] public List<FieldSO> choiceNodeFields = new List<FieldSO>();
        [HideInInspector][Highlightable] public List<FieldSO> choiceFields = new List<FieldSO>();
        [HideInInspector][Highlightable] public List<FieldSO> ifNodeFields = new List<FieldSO>();
        [HideInInspector][Highlightable] public List<FieldSO> eventNodeFields = new List<FieldSO>();
        [HideInInspector][Highlightable] public List<FieldSO> endNodeFields = new List<FieldSO>();

        [HideInInspector][Highlightable] public List<string> languages = new List<string>();
        [HideInInspector] public Language language = Language.English;

        [HideInInspector] public List<FieldSO> fieldSORegistry = new List<FieldSO>();

        private const string SETTINGS_DIRECTORY = "Assets/IndieDevs/Dialogue System/Resources/";
        private const string assetPath = SETTINGS_DIRECTORY + "DialogueSettings.asset";

        public static DialogueSettings GetOrCreateSettings()
        {
#if UNITY_EDITOR
            if (AssetDatabase.IsAssetImportWorkerProcess())
            {
                return null;
            }

            var settings = AssetDatabase.LoadAssetAtPath<DialogueSettings>(assetPath);
            if (settings == null)
            {
                settings = CreateInstance<DialogueSettings>();

                settings.languages = new List<string>
                {
                    "English"
                };

                if (!Directory.Exists(SETTINGS_DIRECTORY))
                {
                    Directory.CreateDirectory(SETTINGS_DIRECTORY);
                }

                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }
            if (settings.languages.Count == 0)
            {
                settings.languages = new List<string>
                {
                    "English"
                };
                AssetDatabase.SaveAssets();
            }
            return settings;
#else
            DialogueSettings settings = Resources.Load<DialogueSettings>("DialogueSettings");

            if (settings == null)
            {
                Debug.LogError("DialogueSettings.asset not found in Resources! Please create it in the Editor first.");
            }

            return settings;
#endif
        }

        public FieldSO GetCustomFieldSO(string label)
        {
            foreach (FieldSO fieldSO in fieldSORegistry)
            {
                if (fieldSO == null)
                {
                    continue;
                }
                else if (fieldSO.label == label)
                {
                    return fieldSO;
                }
            }
            return null;
        }

#if UNITY_EDITOR
        private void ValidateStringList(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = Validator.ValidateIdentifier(list[i]);
            }
            if (languages.Count == 0)
            {
                languages = new List<string>
                {
                    "English"
                };
            }
        }

        private void OnValidate()
        {
            ValidateStringList(dialogueEvents);
            ValidateStringList(dialogueStringVariables);
            ValidateStringList(dialogueIntVariables);
            ValidateStringList(dialogueFloatVariables);
            ValidateStringList(dialogueBoolVariables);
            ValidateStringList(languages);
        }

        public void OnDialogueTreeChanged(DialogueTreeSO dialogueTree)
        {
            this.dialogueTree = dialogueTree;
            Save();
        }

        public void OnLanguageChanged(Language language)
        {
            this.language = language;
            Save();
        }

        private void Save()
        {
            if (!EditorUtility.IsDirty(this))
            {
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}

