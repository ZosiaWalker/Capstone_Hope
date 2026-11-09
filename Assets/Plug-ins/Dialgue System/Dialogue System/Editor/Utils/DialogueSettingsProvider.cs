using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public static class DialogueSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new SettingsProvider("Project/Dialogue System", SettingsScope.Project)
            {
                label = "Dialogue System",
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = DialogueSettings.GetOrCreateSettings();
                    var so = new SerializedObject(settings);

                    rootElement.style.paddingLeft = 4;
                    rootElement.style.paddingTop = 4;
                    rootElement.style.paddingRight = 4;
                    rootElement.style.paddingBottom = 4;

                    // --- Regenerate Banner ---
                    var regenBox = new VisualElement();
                    regenBox.style.display = DisplayStyle.None; // Hidden by default
                    regenBox.style.backgroundColor = new Color(0.25f, 0.15f, 0.05f);
                    regenBox.style.marginBottom = 8;
                    regenBox.style.paddingTop = 6;
                    regenBox.style.paddingBottom = 6;
                    regenBox.style.paddingLeft = 8;
                    regenBox.style.paddingRight = 8;
                    regenBox.style.borderTopLeftRadius = 4;
                    regenBox.style.borderTopRightRadius = 4;
                    regenBox.style.borderBottomLeftRadius = 4;
                    regenBox.style.borderBottomRightRadius = 4;

                    var regenLabel = new Label("Dialogue settings changed. Make sure you regenerate code via 'Tools/Dialogue System/Regenerate Code'.");
                    regenLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                    regenLabel.style.color = Color.yellow;

                    //var regenButton = new Button(() =>
                    //{
                    //    Debug.Log("<b>[Dialogue System]</b> Regenerate dialogue code manually triggered.");
                    //    DialogueSystemCodegen.Generate();
                    //    regenBox.style.display = DisplayStyle.None;
                    //})
                    //{
                    //    text = "Regenerate Now"
                    //};
                    //regenButton.style.marginTop = 4;
                    //regenButton.style.unityTextAlign = TextAnchor.MiddleCenter;

                    regenBox.Add(regenLabel);
                    //regenBox.Add(regenButton);
                    rootElement.Add(regenBox);

                    rootElement.Add(GetLabel("Character Settings", paddingTop: 0));

                    var charactersProp = so.FindProperty("characters");
                    var characters = new PropertyField(charactersProp, "Characters");
                    characters.Bind(so);
                    rootElement.Add(characters);

                    var characterFieldsProp = so.FindProperty("characterFields");
                    var characterFields = new PropertyField(characterFieldsProp, "Character Fields");
                    characterFields.Bind(so);
                    rootElement.Add(characterFields);

                    rootElement.Add(GetLabel("Node Settings"));
                    rootElement.Add(GetLabel("Dialogue Node", fontSize: 13, paddingTop: 4));

                    var dialogueNodeFieldsProp = so.FindProperty("dialogueNodeFields");
                    var dialogueNodeFields = new PropertyField(dialogueNodeFieldsProp, "Dialogue Node Fields");
                    dialogueNodeFields.Bind(so);
                    rootElement.Add(dialogueNodeFields);

                    rootElement.Add(GetLabel("Choice Node", fontSize: 13, paddingTop: 4));

                    var choiceNodeFieldsProp = so.FindProperty("choiceNodeFields");
                    var choiceNodeFields = new PropertyField(choiceNodeFieldsProp, "Choice Node Fields");
                    choiceNodeFields.Bind(so);
                    rootElement.Add(choiceNodeFields);

                    var choiceFieldsProp = so.FindProperty("choiceFields");
                    var choiceFields = new PropertyField(choiceFieldsProp, "Choice Fields");
                    choiceFields.Bind(so);
                    rootElement.Add(choiceFields);

                    rootElement.Add(GetLabel("If Node", fontSize: 13, paddingTop: 4));

                    var ifNodeFieldsProp = so.FindProperty("ifNodeFields");
                    var ifNodeFields = new PropertyField(ifNodeFieldsProp, "If Node Fields");
                    ifNodeFields.Bind(so);
                    rootElement.Add(ifNodeFields);

                    rootElement.Add(GetLabel("Event Node", fontSize: 13, paddingTop: 4));

                    var eventNodeFieldsProp = so.FindProperty("eventNodeFields");
                    var eventNodeFields = new PropertyField(eventNodeFieldsProp, "Event Node Fields");
                    eventNodeFields.Bind(so);
                    rootElement.Add(eventNodeFields);

                    rootElement.Add(GetLabel("End Node", fontSize: 13, paddingTop: 4));

                    var endNodeFieldsProp = so.FindProperty("endNodeFields");
                    var endNodeFields = new PropertyField(endNodeFieldsProp, "End Node Fields");
                    endNodeFields.Bind(so);
                    rootElement.Add(endNodeFields);

                    rootElement.Add(GetLabel("Dialogue Event Settings"));

                    var dialogueEventsProp = so.FindProperty("dialogueEvents");
                    var dialogueEvents = new PropertyField(dialogueEventsProp, "Dialogue Events");
                    dialogueEvents.Bind(so);
                    rootElement.Add(dialogueEvents);

                    rootElement.Add(GetLabel("Dialogue Variable Settings"));

                    var dialogueStringVariablesProp = so.FindProperty("dialogueStringVariables");
                    var dialogueStringVariables = new PropertyField(dialogueStringVariablesProp, "String Variables");
                    dialogueStringVariables.Bind(so);
                    rootElement.Add(dialogueStringVariables);

                    var dialogueIntVariablesProp = so.FindProperty("dialogueIntVariables");
                    var dialogueIntVariables = new PropertyField(dialogueIntVariablesProp, "Int Variables");
                    dialogueIntVariables.Bind(so);
                    rootElement.Add(dialogueIntVariables);

                    var dialogueFloatVariablesProp = so.FindProperty("dialogueFloatVariables");
                    var dialogueFloatVariables = new PropertyField(dialogueFloatVariablesProp, "Float Variables");
                    dialogueFloatVariables.Bind(so);
                    rootElement.Add(dialogueFloatVariables);

                    var dialogueBoolVariablesProp = so.FindProperty("dialogueBoolVariables");
                    var dialogueBoolVariables = new PropertyField(dialogueBoolVariablesProp, "Bool Variables");
                    dialogueBoolVariables.Bind(so);
                    rootElement.Add(dialogueBoolVariables);

                    rootElement.Add(GetLabel("Language Settings"));

                    var languagesProp = so.FindProperty("languages");
                    var languages = new PropertyField(languagesProp, "Languages");
                    languages.Bind(so);
                    rootElement.Add(languages);

                    var trackerElement = new VisualElement();
                    rootElement.Add(trackerElement);

                    trackerElement.TrackSerializedObjectValue(so, _ =>
                    {
                        if (regenBox.style.display != DisplayStyle.Flex)
                        {
                            Logger.LogFieldChangedWarning();
                        }

                        regenBox.style.display = DisplayStyle.Flex;
                    });

                    //rootElement.TrackSerializedObjectValue(so, _ =>
                    //{
                    //    if (regenBox.style.display != DisplayStyle.Flex)
                    //    {
                    //        Logger.LogFieldChangedWarning();
                    //    }
                    //    regenBox.style.display = DisplayStyle.Flex;
                    //});

                    //// Apply button (optional, Unity auto-saves most of this)
                    //var saveButton = new Button(() =>
                    //{
                    //    so.ApplyModifiedProperties();
                    //    EditorUtility.SetDirty(settings);
                    //    AssetDatabase.SaveAssets();
                    //})
                    //{
                    //    text = "Save Settings"
                    //};
                    //rootElement.Add(saveButton);
                }
            };
        }

        private static VisualElement GetLabel(string title, float fontSize = 18, int paddingTop = 8)
        {
            Label label = new Label(title);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.fontSize = fontSize;
            label.style.paddingTop = paddingTop;

            return label;
        }
    }
}
