using UnityEditor;
using UnityEditor.Callbacks;

namespace DialogueSystem
{
    public static class DialogueTreeEditorOpener
    {
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var asset = Selection.activeObject as DialogueTreeSO;

            if (asset != null)
            {
                DialogueTreeEditor window = EditorWindow.GetWindow<DialogueTreeEditor>();
                window.titleContent = new UnityEngine.GUIContent("DialogueTreeEditor");

                window.SetDialogueTree(asset);
                return true;
            }

            return false;
        }
    }
}
