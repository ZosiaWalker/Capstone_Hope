using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogueSystem
{
    public partial class CharacterInternal : ScriptableObject, Character
    {
        public string characterName;
        public CustomFieldSO customFieldSO;

        [HideInInspector] public DialogueTreeSO tree;
        [HideInInspector] public NodeInternal node;

        public static CharacterInternal CreateInstance(DialogueTreeSO tree, NodeInternal node)
        {
            CharacterInternal characterInternal = ScriptableObject.CreateInstance<CharacterInternal>();
            characterInternal.name = characterInternal.GetType().ToString();
            characterInternal.tree = tree;
            characterInternal.node = node;
            return characterInternal;
        }

#if UNITY_EDITOR
        public void OnCharacterChanged(string listener)
        {
            this.characterName = listener;
            Save();
        }

        private void Save()
        {
            if (!EditorUtility.IsDirty(this))
            {
                EditorUtility.SetDirty(this);
            }
        }

        public void CopyFrom(CharacterInternal other)
        {
            characterName = other.characterName;
            customFieldSO.CopyFrom(other.customFieldSO);
        }
#endif

        string Character.CharacterName
        {
            get
            {
                //if (characterName == null && tree != null)
                //{
                //    string fieldName = "Listener";
                //    Logger.LogNullFieldWarning(tree.name, node.name, fieldName);
                //    return null;
                //}
                return characterName;
            }
        }
    }
}
