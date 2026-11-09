using System;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    public abstract class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeView> OnNodeSelected;
        public NodeInternal node;
        protected DialogueSettings dialogueSettings;

        public NodeView(NodeInternal node, string uxml) : base(uxml)
        {
            this.node = node;
            this.viewDataKey = node.guid;
            this.dialogueSettings = DialogueSettings.GetOrCreateSettings();

            style.left = node.position.x;
            style.top = node.position.y;

            if (node.customFieldSO == null)
            {
                CustomFieldSO customFieldSO = CustomFieldSO.CreateInstance();
                node.customFieldSO = customFieldSO;
                AssetDatabase.AddObjectToAsset(customFieldSO, node);
                AssetDatabase.SaveAssets();
            }

            LoadUIElements();
            CreateInputPorts();
            CreateOutputPorts();
        }

        protected abstract void LoadUIElements();
        protected abstract void CreateInputPorts();
        protected abstract void CreateOutputPorts();

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            node.OnPositionChanged(new Vector2(newPos.xMin, newPos.yMin));
        }

        public override void OnSelected()
        {
            base.OnSelected();
            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }
    }
}
