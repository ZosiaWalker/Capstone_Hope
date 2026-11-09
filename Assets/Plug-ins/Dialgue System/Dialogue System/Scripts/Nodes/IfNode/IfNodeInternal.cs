using System.Collections.Generic;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogueSystem
{
    public partial class IfNodeInternal : NodeInternal, IfNode
    {
        public List<IfPortSO> conditions = new List<IfPortSO>();
        public NodeInternal elseChild;


#if UNITY_EDITOR
        public void OnConditionAdded(IfPortSO ifPort)
        {
            conditions.Add(ifPort);
            AssetDatabase.AddObjectToAsset(ifPort, this);
            AssetDatabase.SaveAssets();
        }

        public void OnConditionRemoved(IfPortSO ifPort)
        {
            conditions.Remove(ifPort);
            AssetDatabase.RemoveObjectFromAsset(ifPort);
            AssetDatabase.SaveAssets();
        }

        public void OnElseChildChanged(NodeInternal elseChild)
        {
            this.elseChild = elseChild;
            Save();
        }

        public override void CopyFrom(NodeInternal other)
        {
            base.CopyFrom(other);
            IfNodeInternal otherIfNode = other as IfNodeInternal;
            if (otherIfNode != null)
            {
                foreach (IfPortSO ifPortSO in otherIfNode.conditions)
                {
                    OnConditionAdded(IfPortSO.CreateInstance(tree, this));
                    conditions.Last().CopyFrom(ifPortSO);
                }
            }
        }

        public override void Remove()
        {
            foreach (IfPortSO ifPort in conditions)
            {
                AssetDatabase.RemoveObjectFromAsset(ifPort);
            }
        }
#endif
    }
}
