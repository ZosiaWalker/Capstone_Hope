using System;
using System.Collections.Generic;
using UnityEditor;

namespace DialogueSystem
{
    public static class ClassCodegen
    {
        public static void GenerateNodeClass(string namespaceName, string className, HashSet<FieldSO> fields, HashSet<FieldSO> enums)
        {
            var lines = new List<string>();

            lines.Add(DialogueSystemCodegen.CODEGEN_COMMENT);
            lines.Add("using System.Collections.Generic;");
            lines.Add("using UnityEngine;");
            lines.Add("");
            lines.Add($"namespace {namespaceName}");
            lines.Add("{");
            lines.Add($"    public partial class {className}Internal");
            lines.Add("    {");

            List<string> fieldNames = new List<string>();
            foreach (var fieldSO in fields)
            {
                if (fieldSO == null || string.IsNullOrEmpty(fieldSO.label)) continue;

                string safeName = DialogueSystemCodegen.MakeSafeIdentifier(fieldSO.label);
                if (!fieldNames.Contains(safeName))
                {
                    string typeName = DialogueSystemCodegen.GetCSharpType(fieldSO.fieldType);
                    if (typeName != null)
                    {
                        lines.Add($"        public {typeName} {safeName} => customFieldSO.GetCustomFieldValue<{typeName}>(DialogueFields.{safeName});");
                        fieldNames.Add(safeName);
                    }
                }
            }

            fieldNames = new List<string>();
            foreach (var fieldSO in enums)
            {
                if (fieldSO == null || string.IsNullOrEmpty(fieldSO.label)) continue;

                string safeName = DialogueSystemCodegen.MakeSafeIdentifier(fieldSO.label);
                if (!fieldNames.Contains(safeName))
                {
                    string typeName = fieldSO.label;
                    if (typeName != null)
                    {
                        lines.Add($"        public {typeName} {safeName} => customFieldSO.GetCustomFieldValue<{typeName}>(DialogueFields.{safeName});");
                        fieldNames.Add(safeName);
                    }
                }
            }
            lines.Add("    }");
            lines.Add("}");

            DialogueSystemCodegen.WriteFile(DialogueSystemCodegen.SCRIPTS_GENERATED_FOLDER + $"{className}Internal.Fields.generated.cs", lines);
        }

        public static void GenerateDialogueFields(string namespaceName)
        {
            string[] guids = AssetDatabase.FindAssets("t:FieldSO");

            var fieldSOs = new List<FieldSO>();
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var fieldSO = AssetDatabase.LoadAssetAtPath<FieldSO>(path);
                if (fieldSO != null)
                {
                    fieldSOs.Add(fieldSO);
                }
            }
            DialogueSettings dialogueSettings = DialogueSettings.GetOrCreateSettings();
            dialogueSettings.fieldSORegistry = fieldSOs;
            EditorUtility.SetDirty(dialogueSettings);
            AssetDatabase.SaveAssets();

            var lines = new List<string>();
            lines.Add(DialogueSystemCodegen.CODEGEN_COMMENT);
            lines.Add("using UnityEngine;");
            lines.Add("");
            lines.Add($"namespace {namespaceName}");
            lines.Add("{");
            lines.Add("    public static class DialogueFields");
            lines.Add("    {");

            List<string> fieldNames = new List<string>();
            foreach (var fieldSO in fieldSOs)
            {
                if (!string.IsNullOrEmpty(fieldSO.label))
                {
                    string safeName = DialogueSystemCodegen.MakeSafeIdentifier(fieldSO.label);
                    if (!fieldNames.Contains(safeName))
                    {
                        lines.Add($"        public static FieldSO {safeName} => {nameof(DialogueSettings)}.{nameof(DialogueSettings.GetOrCreateSettings)}().GetCustomFieldSO(\"{fieldSO.label}\");");
                        fieldNames.Add(safeName);
                    }
                }
            }
            lines.Add("    }");
            lines.Add("}");

            DialogueSystemCodegen.WriteFile(DialogueSystemCodegen.SCRIPTS_GENERATED_FOLDER + "DialogueFields.cs", lines);
        }

        public static void GenerateEnumFields(HashSet<FieldSO> enums, string namespaceName)
        {
            var lines = new List<String>();

            lines.Add(DialogueSystemCodegen.CODEGEN_COMMENT);
            lines.Add("using UnityEngine;");
            lines.Add("using UnityEngine.UIElements;");
            lines.Add("");
            lines.Add($"namespace {namespaceName}");
            lines.Add("{");
            lines.Add("    public partial class CustomFieldView");
            lines.Add("    {");

            lines.Add($"        EnumField GetEnumField({nameof(FieldSO)} fieldSO)" + " {");

            List<string> enumNames = new List<string>();
            foreach (var field in enums)
            {
                string enumName = field.label;

                if (!enumNames.Contains(enumName))
                {
                    string propertyName = char.ToUpperInvariant(enumName[0]) + enumName.Substring(1) + "Value";

                    lines.Add($"            if (fieldSO.label == nameof({field.label}))");
                    lines.Add("            {");
                    lines.Add($"                EnumField enumField = new EnumField(\"{enumName}\", customFieldSO.{propertyName});");
                    lines.Add($"                enumField.RegisterValueChangedCallback(evt => customFieldSO.On{enumName}Changed(evt.newValue));");
                    lines.Add("                return enumField;");
                    lines.Add("            }");
                    enumNames.Add(enumName);
                }
            }
            lines.Add("            return null;");
            lines.Add("        }");
            lines.Add("    }");
            lines.Add("}");

            DialogueSystemCodegen.WriteFile(DialogueSystemCodegen.EDITOR_GENERATED_FOLDER + "CustomFieldView.cs", lines);
        }

        public static void GenerateCustomFieldSO(HashSet<FieldSO> enums, string className, string namespaceName, bool isAbstract = false)
        {
            var lines = new List<String>();

            lines.Add(DialogueSystemCodegen.CODEGEN_COMMENT);
            lines.Add("using System;");
            lines.Add("using UnityEngine;");
            lines.Add("");
            lines.Add($"namespace {namespaceName}");
            lines.Add("{");

            if (isAbstract)
            {
                lines.Add($"    public abstract partial class {className}");
            }
            else
            {
                lines.Add($"    public partial class {className}");
            }
            lines.Add("    {");

            List<string> enumNames = new List<string>();
            foreach (var enumField in enums)
            {
                string enumName = enumField.label;
                if (!enumNames.Contains(enumName))
                {
                    string variableName = char.ToLowerInvariant(enumName[0]) + enumName.Substring(1) + "Value";
                    string propertyName = char.ToUpperInvariant(enumName[0]) + enumName.Substring(1) + "Value";

                    lines.Add($"        [HideInInspector] public {enumName} {variableName};");
                    lines.Add($"        public {enumName} {propertyName} => {variableName};");
                    enumNames.Add(enumName);
                }
            }

            if (enums.Count > 0)
            {
                lines.Add("");
                lines.Add("#if UNITY_EDITOR");
                enumNames.Clear();
                foreach (var enumField in enums)
                {
                    string enumName = enumField.label;
                    if (!enumNames.Contains(enumName))
                    {
                        string variableName = char.ToLowerInvariant(enumName[0]) + enumName.Substring(1) + "Value";

                        lines.Add($"        public void On{enumName}Changed(Enum value)");
                        lines.Add("        {");
                        lines.Add($"            {variableName} = ({enumName})value;");
                        lines.Add("            Save();");
                        lines.Add("        }");
                        enumNames.Add(enumName);
                    }
                }
                lines.Add("#endif");
                lines.Add("");
            }

            string fieldSOVariable = "fieldSO";
            lines.Add($"        public {nameof(Enum)} GetEnumValue({nameof(FieldSO)} {fieldSOVariable})");
            lines.Add("        {");

            enumNames.Clear();
            foreach (var field in enums)
            {
                string enumName = field.label;
                if (!enumNames.Contains(enumName))
                {
                    lines.Add($"            if ({fieldSOVariable}.label == nameof({enumName}))");
                    lines.Add("            {");
                    lines.Add($"                return {enumName}Value;");
                    lines.Add("            }");
                    enumNames.Add(enumName);
                }
            }
            lines.Add("            return null;");
            lines.Add("        }");

            string otherVariable = "other";
            lines.Add($"        public void CopyEnumsFrom({nameof(CustomFieldSO)} {otherVariable})");
            lines.Add("        {");

            enumNames.Clear();
            foreach (var field in enums)
            {
                string enumName = field.label;
                if (!enumNames.Contains(enumName))
                {
                    string variableName = char.ToLowerInvariant(enumName[0]) + enumName.Substring(1) + "Value";
                    lines.Add($"            {variableName} = {otherVariable}.{variableName};");
                    enumNames.Add(enumName);
                }
            }
            lines.Add("        }");
            lines.Add("    }");
            lines.Add("}");

            DialogueSystemCodegen.WriteFile(DialogueSystemCodegen.SCRIPTS_GENERATED_FOLDER + $"{className}.Variables.cs", lines);
        }
    }
}
