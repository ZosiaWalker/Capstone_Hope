using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueTreeExporter : Editor
    {
        [MenuItem("Assets/Export to CSV", true)]
        public static bool ValidateExportDialogueTrees()
        {
            foreach (var obj in Selection.objects)
            {
                if (!(obj is DialogueTreeSO))
                {
                    return false;
                }
            }
            return Selection.objects.Length > 0;
        }

        [MenuItem("Assets/Export to CSV")]
        public static void ExportDialogueTrees()
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select Folder to Save CSVs", Application.dataPath, "");

            if (string.IsNullOrEmpty(folderPath))
            {
                Debug.Log("Export cancelled.");
                return;
            }

            List<string> failedExports = new List<string>();

            DialogueTreeCSVManager dialogueTreeCSVManager = new DialogueTreeCSVManager();
            foreach (var obj in Selection.objects)
            {
                if (obj is DialogueTreeSO dialogueTree)
                {
                    try
                    {
                        string filePath = Path.Combine(folderPath, $"{dialogueTree.name}.csv");
                        dialogueTreeCSVManager.ExportCSV(dialogueTree, filePath);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"Failed to export {obj.name}: {ex.Message}");
                        failedExports.Add(obj.name);
                    }
                }
            }

            if (failedExports.Count > 0)
            {
                Debug.LogWarning($"Failed to export {failedExports.Count} dialogue trees: {string.Join(", ", failedExports)}");
            }
            else
            {
                Debug.Log("All selected dialogue trees exported successfully.");
            }
        }
    }
}
