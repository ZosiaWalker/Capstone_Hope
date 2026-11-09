using System.Collections.Generic;

namespace DialogueSystem
{
    public static class EnumCodegen
    {
        public static void GenerateEnum(string namespaceName, string enumName, List<string> values)
        {
            GenerateEnum(namespaceName, enumName, values, DialogueSystemCodegen.SCRIPTS_GENERATED_ENUMS_FOLDER);
        }

        public static void GenerateEnum(string namespaceName, string enumName, List<string> values, string folder)
        {
            //if (values == null || values.Count == 0)
            //    values = new List<string> { "None" };

            //for (int i = 0; i < values.Count; i++)
            //{
            //    string clean = values[i]
            //        .Replace(" ", "_")
            //        .Replace("-", "_")
            //        .Replace(".", "_");
            //    if (string.IsNullOrWhiteSpace(clean))
            //        clean = "None";
            //    values[i] = clean;
            //}

            var lines = new List<string>();

            lines.Add(DialogueSystemCodegen.CODEGEN_COMMENT);
            lines.Add($"namespace {namespaceName}");
            lines.Add("{");
            lines.Add($"    public enum {enumName}");
            lines.Add("    {");

            List<string> valueNames = new List<string>();
            foreach (var value in values)
            {
                if (!valueNames.Contains(value))
                {
                    lines.Add($"        {value},");
                    valueNames.Add(value);
                }
            }

            lines.Add("    }");
            lines.Add("}");

            DialogueSystemCodegen.WriteFile(folder + $"{enumName}.cs", lines);
        }
    }
}
