using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue Tree", menuName = "Dialogue System/Dialogue Tree", order = -3)]
    public class DialogueTreeSO : ScriptableObject
    {
        [Identifier]
        [SerializeField]
        private string dialogueID;
        public string DialogueID
        {
            get
            {
                return dialogueID;
            }
        }
        [HideInInspector]
        public StartNodeInternal startNode;
        [HideInInspector]
        public List<NodeInternal> nodes = new List<NodeInternal>();

#if UNITY_EDITOR
        public Action<GraphElement> RemoveGraphElement;
#endif

#if UNITY_EDITOR
        public NodeInternal CreateNode(System.Type type)
        {
            CustomFieldSO customFieldSO = CustomFieldSO.CreateInstance();
            NodeInternal node = ScriptableObject.CreateInstance(type) as NodeInternal;
            node.customFieldSO = customFieldSO;
            node.name = GetNodeName(type);
            node.tree = this;

            node.guid = GUID.Generate().ToString();

            nodes.Add(node);

            AssetDatabase.AddObjectToAsset(customFieldSO, this);
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            return node;
        }

        public void OnStartNodeChanged(StartNodeInternal startNode)
        {
            this.startNode = startNode;
            Save();
        }

        private void Save()
        {
            if (!EditorUtility.IsDirty(this))
            {
                EditorUtility.SetDirty(this);
            }
        }

        public void DeleteNode(NodeInternal node)
        {
            node.Remove();
            nodes.Remove(node);
            if (node.customFieldSO != null)
            {
                AssetDatabase.RemoveObjectFromAsset(node.customFieldSO);
            }
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(NodeInternal parent, NodeInternal child)
        {
            StartNodeInternal startNode = parent as StartNodeInternal;
            if (startNode)
            {
                startNode.OnChildChanged(child);
            }

            DialogueNodeInternal dialogueNode = parent as DialogueNodeInternal;
            if (dialogueNode)
            {
                dialogueNode.OnChildChanged(child);
            }

            EventNodeInternal eventNode = parent as EventNodeInternal;
            if (eventNode)
            {
                eventNode.OnChildChanged(child);
            }
        }

        public void RemoveChild(NodeInternal parent, int index)
        {
            StartNodeInternal startNode = parent as StartNodeInternal;
            if (startNode)
            {
                startNode.OnChildChanged(null);
            }

            DialogueNodeInternal dialogueNode = parent as DialogueNodeInternal;
            if (dialogueNode)
            {
                dialogueNode.OnChildChanged(null);
            }

            ChoiceNodeInternal choiceNode = parent as ChoiceNodeInternal;
            if (choiceNode)
            {
                ChoiceInternal choicePort = choiceNode.choices[index];
                choicePort.OnChildChanged(null);
            }

            IfNodeInternal ifNode = parent as IfNodeInternal;
            if (ifNode)
            {
                if (index == -1)
                {
                    ifNode.OnElseChildChanged(null);
                }
                else
                {
                    IfPortSO ifPort = ifNode.conditions[index];
                    ifPort.OnChildChanged(null);
                }
            }

            EventNodeInternal eventNode = parent as EventNodeInternal;
            if (eventNode)
            {
                eventNode.OnChildChanged(null);
            }
        }
#endif

        public List<NodeInternal> GetChildren(NodeInternal parent)
        {
            List<NodeInternal> children = new List<NodeInternal>();

            StartNodeInternal startNode = parent as StartNodeInternal;
            if (startNode)
            {
                children.Add(startNode.child);
            }

            DialogueNodeInternal dialogueNode = parent as DialogueNodeInternal;
            if (dialogueNode)
            {
                children.Add(dialogueNode.child);
            }

            ChoiceNodeInternal choiceNode = parent as ChoiceNodeInternal;
            if (choiceNode)
            {
                foreach (ChoiceInternal choicePort in choiceNode.choices)
                {
                    if (choicePort.child != null && !children.Contains(choicePort.child))
                    {
                        children.Add(choicePort.child);
                    }
                }
            }

            IfNodeInternal ifNode = parent as IfNodeInternal;
            if (ifNode)
            {
                children.Add(ifNode.elseChild);
                foreach (IfPortSO ifPort in ifNode.conditions)
                {
                    if (ifPort.child != null && !children.Contains(ifPort.child))
                    {
                        children.Add(ifPort.child);
                    }
                }
            }

            EventNodeInternal eventNode = parent as EventNodeInternal;
            if (eventNode)
            {
                children.Add(eventNode.child);
            }

            return children;
        }

        public string GetNodeName(System.Type type)
        {
            if (type.Equals(typeof(StartNodeInternal)))
            {
                return "Start Node";
            }
            else if (type.Equals(typeof(ChoiceNodeInternal)))
            {
                return "Choice Node";
            }
            else if (type.Equals(typeof(DialogueNodeInternal)))
            {
                return "Dialogue Node";
            }
            else if (type.Equals(typeof(EventNodeInternal)))
            {
                return "Event Node";
            }
            else if (type.Equals(typeof(IfNodeInternal)))
            {
                return "If Node";
            }
            else if (type.Equals(typeof(EndNodeInternal)))
            {
                return "End Node";
            }
            return type.Name;
        }
    }
}
