using System.Collections.Generic;

namespace DialogueSystem
{
    public static class InterfaceCodegen
    {
        public static void GenerateNodeInterface(string namespaceName, string interfaceName, HashSet<FieldSO> fields, HashSet<FieldSO> enums)
        {
            var lines = new List<string>();

            lines.Add(DialogueSystemCodegen.CODEGEN_COMMENT);
            lines.Add("using System.Collections.Generic;");
            lines.Add("using UnityEngine;");
            lines.Add("");
            lines.Add($"namespace {namespaceName}");
            lines.Add("{");
            lines.Add($"    public partial interface {interfaceName}");
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
                        lines.Add($"        public {typeName} {safeName} {{ get; }}");
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
                        lines.Add($"        public {typeName} {safeName} {{ get; }}");
                        fieldNames.Add(safeName);
                    }
                }
            }

            lines.Add("    }");
            lines.Add("}");

            DialogueSystemCodegen.WriteFile(DialogueSystemCodegen.SCRIPTS_GENERATED_FOLDER + $"{interfaceName}.Fields.generated.cs", lines);
        }
    }
}
