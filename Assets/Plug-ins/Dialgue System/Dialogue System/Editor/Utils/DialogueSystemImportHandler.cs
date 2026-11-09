using UnityEditor;

namespace DialogueSystem
{
    public class DialogueSystemImportHandler : AssetPostprocessor
    {
        //static void OnPostprocessAllAssets(
        //    string[] importedAssets,
        //    string[] deletedAssets,
        //    string[] movedAssets,
        //    string[] movedFromAssetPaths)
        //{
        //    //bool importedDialogueSystem = false;

        //    //foreach (string assetPath in importedAssets)
        //    //{
        //    //    if (assetPath.StartsWith("Assets/IndieDevs/Dialogue System"))
        //    //    {
        //    //        importedDialogueSystem = true;
        //    //        break;
        //    //    }
        //    //}

        //    //if (!importedDialogueSystem)
        //    //    return;

        //    if (!Directory.Exists(DialogueSystemCodegen.SCRIPTS_GENERATED_FOLDER) || !Directory.Exists(DialogueSystemCodegen.EDITOR_GENERATED_FOLDER))
        //    {
        //        Debug.Log("[Dialogue System] First import detected. Running code generation...");
        //        DialogueSystemCodegen.Generate();
        //    }
        //}
    }
}
