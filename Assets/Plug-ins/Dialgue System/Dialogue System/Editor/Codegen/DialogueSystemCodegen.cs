using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    public static class DialogueSystemCodegen
    {
        public static string CODEGEN_COMMENT = "// Auto-generated code, do not modify by hand";

        public static string SCRIPTS_FOLDER = "Assets/IndieDevs/Dialogue System/Scripts/";
        public static string SCRIPTS_GENERATED_FOLDER = SCRIPTS_FOLDER + "Generated/";
        public static string SCRIPTS_GENERATED_ENUMS_FOLDER = SCRIPTS_GENERATED_FOLDER + "Enums/";
        public static string SCRIPTS_ENUMS_FOLDER = SCRIPTS_FOLDER + "Enums/";

        public static string EDITOR_FOLDER = "Assets/IndieDevs/Dialogue System/Editor/";
        public static string EDITOR_GENERATED_FOLDER = EDITOR_FOLDER + "Generated/";

        [MenuItem("Tools/Dialogue System/Regenerate Code")]
        public static void Generate()
        {
            DeleteFolder(SCRIPTS_GENERATED_FOLDER);
            CreateFolder(SCRIPTS_GENERATED_FOLDER);
            CreateFolder(SCRIPTS_GENERATED_ENUMS_FOLDER);

            DialogueSettings settings = DialogueSettings.GetOrCreateSettings();
            if (settings == null)
            {
                Debug.LogError("DialogueSettings not found!");
                return;
            }

            string namespaceName = nameof(DialogueSystem);
            ClassCodegen.GenerateDialogueFields(namespaceName);

            HashSet<FieldSO> dialogueNodeFields = new HashSet<FieldSO>();
            HashSet<FieldSO> choiceNodeFields = new HashSet<FieldSO>();
            HashSet<FieldSO> choiceFields = new HashSet<FieldSO>();
            HashSet<FieldSO> ifNodeFields = new HashSet<FieldSO>();
            HashSet<FieldSO> eventNodeFields = new HashSet<FieldSO>();
            HashSet<FieldSO> endNodeFields = new HashSet<FieldSO>();
            HashSet<FieldSO> characterFields = new HashSet<FieldSO>();

            HashSet<FieldSO> dialogueNodeEnums = new HashSet<FieldSO>();
            HashSet<FieldSO> choiceNodeEnums = new HashSet<FieldSO>();
            HashSet<FieldSO> choiceEnums = new HashSet<FieldSO>();
            HashSet<FieldSO> ifNodeEnums = new HashSet<FieldSO>();
            HashSet<FieldSO> eventNodeEnums = new HashSet<FieldSO>();
            HashSet<FieldSO> endNodeEnums = new HashSet<FieldSO>();
            HashSet<FieldSO> characterEnums = new HashSet<FieldSO>();

            GenerateHashSets(settings.dialogueNodeFields, dialogueNodeFields, dialogueNodeEnums);
            GenerateHashSets(settings.choiceNodeFields, choiceNodeFields, choiceNodeEnums);
            GenerateHashSets(settings.choiceFields, choiceFields, choiceEnums);
            GenerateHashSets(settings.ifNodeFields, ifNodeFields, ifNodeEnums);
            GenerateHashSets(settings.eventNodeFields, eventNodeFields, eventNodeEnums);
            GenerateHashSets(settings.endNodeFields, endNodeFields, endNodeEnums);
            GenerateHashSets(settings.characterFields, characterFields, characterEnums);

            List<string> fieldNames = new List<string>();
            foreach (var field in dialogueNodeEnums)
            {
                if (!fieldNames.Contains(field.label))
                {
                    EnumCodegen.GenerateEnum(namespaceName, field.label, field.enumChoices);
                    fieldNames.Add(field.label);
                }
            }
            foreach (var field in choiceNodeEnums)
            {
                if (!fieldNames.Contains(field.label))
                {
                    EnumCodegen.GenerateEnum(namespaceName, field.label, field.enumChoices);
                    fieldNames.Add(field.label);
                }
            }
            foreach (var field in choiceEnums)
            {
                if (!fieldNames.Contains(field.label))
                {
                    EnumCodegen.GenerateEnum(namespaceName, field.label, field.enumChoices);
                    fieldNames.Add(field.label);
                }
            }
            foreach (var field in ifNodeEnums)
            {
                if (!fieldNames.Contains(field.label))
                {
                    EnumCodegen.GenerateEnum(namespaceName, field.label, field.enumChoices);
                    fieldNames.Add(field.label);
                }
            }
            foreach (var field in eventNodeEnums)
            {
                if (!fieldNames.Contains(field.label))
                {
                    EnumCodegen.GenerateEnum(namespaceName, field.label, field.enumChoices);
                    fieldNames.Add(field.label);
                }
            }
            foreach (var field in characterEnums)
            {
                if (!fieldNames.Contains(field.label))
                {
                    EnumCodegen.GenerateEnum(namespaceName, field.label, field.enumChoices);
                    fieldNames.Add(field.label);
                }
            }

            HashSet<FieldSO> enums = new HashSet<FieldSO>();
            enums.UnionWith(dialogueNodeEnums);
            enums.UnionWith(choiceNodeEnums);
            enums.UnionWith(choiceEnums);
            enums.UnionWith(ifNodeEnums);
            enums.UnionWith(eventNodeEnums);
            enums.UnionWith(endNodeEnums);
            enums.UnionWith(characterEnums);

            ClassCodegen.GenerateCustomFieldSO(enums, "CustomFieldSO", namespaceName);
            ClassCodegen.GenerateEnumFields(enums, namespaceName);

            string dialogueNode = "DialogueNode";
            string choiceNode = "ChoiceNode";
            string ifNode = "IfNode";
            string eventNode = "EventNode";
            string endNode = "EndNode";

            InterfaceCodegen.GenerateNodeInterface(namespaceName, dialogueNode, dialogueNodeFields, dialogueNodeEnums);
            ClassCodegen.GenerateNodeClass(namespaceName, dialogueNode, dialogueNodeFields, dialogueNodeEnums);

            InterfaceCodegen.GenerateNodeInterface(namespaceName, choiceNode, choiceNodeFields, choiceNodeEnums);
            ClassCodegen.GenerateNodeClass(namespaceName, choiceNode, choiceNodeFields, choiceNodeEnums);

            InterfaceCodegen.GenerateNodeInterface(namespaceName, "Choice", choiceFields, choiceEnums);
            ClassCodegen.GenerateNodeClass(namespaceName, "Choice", choiceFields, choiceEnums);

            InterfaceCodegen.GenerateNodeInterface(namespaceName, ifNode, ifNodeFields, ifNodeEnums);
            ClassCodegen.GenerateNodeClass(namespaceName, ifNode, ifNodeFields, ifNodeEnums);

            InterfaceCodegen.GenerateNodeInterface(namespaceName, eventNode, eventNodeFields, eventNodeEnums);
            ClassCodegen.GenerateNodeClass(namespaceName, eventNode, eventNodeFields, eventNodeEnums);

            InterfaceCodegen.GenerateNodeInterface(namespaceName, endNode, endNodeFields, endNodeEnums);
            ClassCodegen.GenerateNodeClass(namespaceName, endNode, endNodeFields, endNodeEnums);

            InterfaceCodegen.GenerateNodeInterface(namespaceName, "Character", characterFields, characterEnums);
            ClassCodegen.GenerateNodeClass(namespaceName, "Character", characterFields, characterEnums);

            //EnumCodegen.GenerateEnum(namespaceName, nameof(DialogueEvent), settings.dialogueEvents);
            //EnumCodegen.GenerateEnum(namespaceName, nameof(DialogueStringVariable), settings.dialogueStringVariables);
            //EnumCodegen.GenerateEnum(namespaceName, nameof(DialogueIntVariable), settings.dialogueIntVariables);
            //EnumCodegen.GenerateEnum(namespaceName, nameof(DialogueFloatVariable), settings.dialogueFloatVariables);
            //EnumCodegen.GenerateEnum(namespaceName, nameof(DialogueBoolVariable), settings.dialogueBoolVariables);

            EnumCodegen.GenerateEnum(namespaceName, "DialogueEvent", settings.dialogueEvents, SCRIPTS_ENUMS_FOLDER);
            EnumCodegen.GenerateEnum(namespaceName, "DialogueStringVariable", settings.dialogueStringVariables, SCRIPTS_ENUMS_FOLDER);
            EnumCodegen.GenerateEnum(namespaceName, "DialogueIntVariable", settings.dialogueIntVariables, SCRIPTS_ENUMS_FOLDER);
            EnumCodegen.GenerateEnum(namespaceName, "DialogueFloatVariable", settings.dialogueFloatVariables, SCRIPTS_ENUMS_FOLDER);
            EnumCodegen.GenerateEnum(namespaceName, "DialogueBoolVariable", settings.dialogueBoolVariables, SCRIPTS_ENUMS_FOLDER);
            EnumCodegen.GenerateEnum(namespaceName, "Language", settings.languages, SCRIPTS_ENUMS_FOLDER);

            AssetDatabase.SaveAssets();
            EditorApplication.delayCall += () =>
            {
                AssetDatabase.Refresh();
                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
            };
        }

        private static void GenerateHashSets(List<FieldSO> fields, HashSet<FieldSO> nodeFields, HashSet<FieldSO> nodeEnums)
        {
            foreach (FieldSO fieldSO in fields)
            {
                if (fieldSO == null) continue;
                else if (fieldSO.fieldType == CustomFieldType.Enum)
                {
                    nodeEnums.Add(fieldSO);
                }
                else
                {
                    nodeFields.Add(fieldSO);
                }
            }
        }

        public static void DeleteFolder(string relativePath)
        {
            string fullPath = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - 6), relativePath);

            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
                string metaFile = fullPath + ".meta";
                if (File.Exists(metaFile))
                {
                    File.Delete(metaFile);
                }

                AssetDatabase.Refresh();
            }
        }

        private static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void WriteFile(string path, List<string> lines)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllLines(path, lines);
        }

        public static string MakeSafeIdentifier(string name)
        {
            string safe = new string(System.Array.ConvertAll(name.ToCharArray(), c =>
                char.IsLetterOrDigit(c) ? c : '_'));
            if (string.IsNullOrEmpty(safe))
                safe = "_";
            if (char.IsDigit(safe[0]))
                safe = "_" + safe;
            return safe;
        }

        public static string GetCSharpType(CustomFieldType fieldType)
        {
            return fieldType switch
            {
                CustomFieldType.String => "string",
                CustomFieldType.Int => "int",
                CustomFieldType.Float => "float",
                CustomFieldType.Bool => "bool",
                CustomFieldType.Vector2 => nameof(Vector2),
                CustomFieldType.Vector3 => nameof(Vector3),
                CustomFieldType.Sprite => nameof(Sprite),
                CustomFieldType.GameObject => nameof(GameObject),
                CustomFieldType.AudioClip => nameof(AudioClip),
                CustomFieldType.DialogueTree => nameof(DialogueTreeSO),
                _ => null
            };
        }
    }
}
