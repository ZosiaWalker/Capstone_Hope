using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueTreeCSVManager
    {
        public void ImportCSV(DialogueTreeSO dialogueTree)
        {
            string path = EditorUtility.OpenFilePanel(
                "Import Dialogue Tree as CSV",
                Application.dataPath,
                "csv"
            );
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string[] csvLines = File.ReadAllLines(path, Encoding.UTF8);
            string[] headers = ParseCSVRow(csvLines[0]);
            string[] languages = headers[1..];

            Dictionary<string, string[]> rows = new Dictionary<string, string[]>();

            for (int i = 1; i < csvLines.Length; i++)
            {
                string[] cols = ParseCSVRow(csvLines[i]);
                string nodeId = cols[0];
                rows[nodeId] = cols[1..];
            }

            foreach (KeyValuePair<string, string[]> keyValuePair in rows)
            {
                string nodeId = keyValuePair.Key;
                string[] row = keyValuePair.Value;

                NodeInternal node = dialogueTree.nodes.OfType<NodeInternal>().FirstOrDefault(node => node.guid == nodeId);

                if (node == null)
                {
                    if (!nodeId.Contains("Choice"))
                    {
                        Debug.LogWarning($"Node with ID {nodeId} not found. Skipping row.");
                    }
                    continue;
                }

                for (int j = 0; j < languages.Length; j++)
                {
                    if (Enum.TryParse(languages[j], out Language language))
                    {
                        string message = row[j];

                        if (node is DialogueNodeInternal dialogueNode)
                        {
                            dialogueNode.OnMessageChanged(language, message);
                        }
                        else if (node is ChoiceNodeInternal choiceNode)
                        {
                            choiceNode.OnMessageChanged(language, message);

                            for (int k = 0; k < choiceNode.choices.Count; k++)
                            {
                                ChoiceInternal choice = choiceNode.choices[k];
                                int index = k + 1;

                                string choiceId = $"{nodeId}Choice{index}";
                                string[] choiceRow = rows[choiceId];

                                string choiceMessage = choiceRow[j];

                                choice.OnMessageChanged(language, choiceMessage);
                            }
                        }
                    }
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"CSV Imported successfully from {path}");
        }

        private string[] ParseCSVRow(string csvLine)
        {
            List<string> fields = new List<string>();
            bool insideQuotes = false;
            string currentField = "";

            foreach (char c in csvLine)
            {
                if (c == '"' && !insideQuotes)
                {
                    insideQuotes = true;
                }
                else if (c == '"' && insideQuotes)
                {
                    insideQuotes = false;
                }
                else if (c == ',' && !insideQuotes)
                {
                    fields.Add(currentField);
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }
            fields.Add(currentField);

            return fields.ToArray();
        }

        public void ExportCSV(DialogueTreeSO dialogueTree)
        {
            string path = EditorUtility.SaveFilePanel(
                "Save Dialogue Tree as CSV",
                Application.dataPath,
                $"{dialogueTree.name}.csv",
                "csv"
            );
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            ExportCSVInternal(dialogueTree, path);
            OpenInFileExplorer(path);
        }

        public void ExportCSV(DialogueTreeSO dialogueTree, string filePath)
        {
            ExportCSVInternal(dialogueTree, filePath);
        }

        private void ExportCSVInternal(DialogueTreeSO dialogueTree, string path)
        {
            List<string> languages = Enum.GetNames(typeof(Language)).ToList();
            List<string> csvLines = new List<string>();

            string header = "Node ID," + string.Join(",", languages);
            csvLines.Add(header);

            csvLines.AddRange(GetMessages(dialogueTree, languages));

            File.WriteAllLines(path, csvLines, new UTF8Encoding(true));
            Debug.Log($"CSV Exported successfully to {path}");
        }

        private List<string> GetMessages(DialogueTreeSO dialogueTree, List<string> languages)
        {
            List<string> messages = new List<string>();
            foreach (NodeInternal node in dialogueTree.nodes)
            {
                if (node is DialogueNodeInternal dialogueNode)
                {
                    string row = dialogueNode.guid;
                    foreach (string stringLanguage in languages)
                    {
                        if (Enum.TryParse(stringLanguage, out Language language))
                        {
                            string message = dialogueNode.GetMessageInternal(language);
                            row += $",{EscapeCSVValue(message)}";
                        }
                    }
                    messages.Add(row);
                }
                else if (node is ChoiceNodeInternal choiceNode)
                {
                    string row = choiceNode.guid;
                    foreach (string stringLanguage in languages)
                    {
                        if (Enum.TryParse(stringLanguage, out Language language))
                        {
                            string message = choiceNode.GetMessageInternal(language);
                            row += $",{EscapeCSVValue(message)}";
                        }
                    }
                    messages.Add(row);

                    for (int i = 0; i < choiceNode.choices.Count; i++)
                    {
                        string choiceId = $"{choiceNode.guid}Choice{i + 1}";
                        string choiceRow = choiceId;
                        foreach (string stringLanguage in languages)
                        {
                            if (Enum.TryParse(stringLanguage, out Language language))
                            {
                                string message = choiceNode.choices[i].GetMessageInternal(language);
                                choiceRow += $",{EscapeCSVValue(message)}";
                            }
                        }
                        messages.Add(choiceRow);
                    }
                }
            }
            return messages;
        }

        private string EscapeCSVValue(string value)
        {
            if (value.Contains(",") || value.Contains("\""))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }
            return value;
        }

        private void OpenInFileExplorer(string filePath)
        {
            string folderPath = Path.GetDirectoryName(filePath);

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                System.Diagnostics.Process.Start("explorer.exe", folderPath);
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                System.Diagnostics.Process.Start("open", folderPath);
            }
            else if (Application.platform == RuntimePlatform.LinuxEditor)
            {
                System.Diagnostics.Process.Start("xdg-open", folderPath);
            }
        }
    }
}