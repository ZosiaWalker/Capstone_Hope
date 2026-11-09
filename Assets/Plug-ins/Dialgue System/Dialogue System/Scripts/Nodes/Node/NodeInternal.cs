using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogueSystem
{
    public abstract partial class NodeInternal : ScriptableObject, Node
    {
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public DialogueTreeSO tree;
        public CustomFieldSO customFieldSO;

        [ReadOnly]
        public string nodeID;

#if UNITY_EDITOR
        private void OnEnable()
        {
            if (string.IsNullOrEmpty(nodeID))
            {
                nodeID = Guid.NewGuid().ToString();
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        public void OnPositionChanged(Vector2 position)
        {
            this.position = position;
            Save();
        }
        public void Save()
        {
            if (!EditorUtility.IsDirty(this))
            {
                EditorUtility.SetDirty(this);
            }
        }

        public virtual void CopyFrom(NodeInternal other)
        {
            this.customFieldSO.CopyFrom(other.customFieldSO);
        }

        public abstract void Remove();
#endif
        string Node.nodeID
        {
            get
            {
                return nodeID;
            }
        }
    }
}
