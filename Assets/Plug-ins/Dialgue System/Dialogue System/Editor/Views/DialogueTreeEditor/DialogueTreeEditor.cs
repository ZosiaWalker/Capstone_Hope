using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueTreeEditor : EditorWindow
    {
        private DialogueTreeSO selectedTree;
        DialogueTreeView dialogueTreeView;

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        private Label treeLabel;
        private EnumField languages;
        private Button exportCSVBtn;
        private Button importCSVBtn;
        private string styleSheetPath;

        private DialogueTreeCSVManager dialogueTreeCSVManager = new DialogueTreeCSVManager();

        private DialogueSettings dialogueSettings;

        [MenuItem("Window/Dialogue System/Dialogue Tree Editor")]
        public static void OpenWindow()
        {
            DialogueTreeEditor wnd = GetWindow<DialogueTreeEditor>();
            wnd.titleContent = new GUIContent("Dialogue Tree Editor");
        }

        void OnEnable()
        {
            if (selectedTree == null)
            {
                dialogueSettings = DialogueSettings.GetOrCreateSettings();
                selectedTree = dialogueSettings.dialogueTree;
            }
            if (dialogueTreeView == null)
            {
                CreateGUI();
                SetDialogueTree(selectedTree);
            }
        }

        public void CreateGUI()
        {
            if (dialogueTreeView != null)
            {
                return;
            }
            VisualElement root = rootVisualElement;

            m_VisualTreeAsset.CloneTree(root);

            AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            styleSheetPath = Path.ChangeExtension(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)), "uss");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);

            root.styleSheets.Add(styleSheet);

            dialogueTreeView = root.Q<DialogueTreeView>();

            treeLabel = root.Q<Label>("tree-label");

            languages = root.Q<EnumField>("language");
            if (languages != null)
            {
                languages.Init(dialogueSettings.language);
                languages.value = dialogueSettings.language;
            }

            exportCSVBtn = root.Q<Button>("export-btn");
            if (exportCSVBtn != null)
            {
                exportCSVBtn.clickable.clicked += () =>
                {
                    dialogueTreeCSVManager.ExportCSV(selectedTree);
                };
            }

            importCSVBtn = root.Q<Button>("import-btn");
            if (importCSVBtn != null)
            {
                importCSVBtn.clickable.clicked += () =>
                {
                    dialogueTreeCSVManager.ImportCSV(selectedTree);
                    SetDialogueTree(selectedTree);
                };
            }

            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            DialogueTreeSO tree = Selection.activeObject as DialogueTreeSO;

            if (tree != null && AssetDatabase.Contains(tree))
            {
                SetDialogueTree(tree);
                // if (selectedTree != tree)
                // {
                //     SetDialogueTree(tree);
                // }
            }
            else if (selectedTree == null)
            {
                ClearEditorView();
            }
            SetTreeLabel();
        }

        private void SetTreeLabel()
        {
            if (treeLabel != null)
            {
                if (selectedTree != null)
                {
                    treeLabel.text = selectedTree.name;
                }
                else
                {
                    treeLabel.text = "NO TREE SELECTED!";
                }
            }
        }

        private void ClearEditorView()
        {
            if (dialogueTreeView != null)
            {
                dialogueTreeView.ClearView();
            }
            selectedTree = null;
        }

        public void SetDialogueTree(DialogueTreeSO tree)
        {
            if (tree == null)
            {
                return;
            }
            if (dialogueSettings != null)
            {
                dialogueSettings.OnDialogueTreeChanged(tree);
            }
            selectedTree = tree;
            if (dialogueTreeView != null)
            {
                dialogueTreeView.PopulateView(tree, styleSheetPath);
            }
            else
            {
                Debug.LogWarning("An error occured. Please close the DialogueTreeEditor tab and then re-open it to fix the issue.");
            }
            SetTreeLabel();
            //if (languages != null)
            //{
            //    languages.SetValueWithoutNotify(dialogueSettings.language);
            //}
            languages.RegisterValueChangedCallback(evt =>
            {
                dialogueSettings.OnLanguageChanged((Language)evt.newValue);
                SetDialogueTree(selectedTree);
            });
        }
    }
}
